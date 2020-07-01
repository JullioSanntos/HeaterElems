using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;

//namespace Common.Utilities
namespace HeaterElems.Common
{
    /// <summary>
    /// Provides a way for clients to be notified when an specified time was reached.
    /// It also provides a series of progress notifications through <see cref="RunningTime"/> (as in Tick/Tock).
    /// 
    /// The timer completed notification is done through any one of two approaches:
    ///     1) By awaiting the <see cref="StartAsync"/> call to be returned;
    ///     2) By subscribing to the <see cref="RunCompleted"/> event.
    /// 
    /// Progress notifications is done through <see cref="INotifyPropertyChanged.PropertyChanged"/> events which
    /// identifies property <see cref="RunningTime"/> as having changed.
    ///
    /// <example>
    /// In this example ProgressiveTime raises <see cref="RunCompleted"/> event after 2 seconds after
    /// it raises <see cref="RunningTime"/> PropertyChange event every 1/2 second.
    /// <code>
    ///     var sut = new ProgressiveTimer();
    ///     sut.PropertyChanged += (s, e) => {if (e.PropertyName == nameof(sut.ProgressTick)) Console.WriteLine(sut.ProgressTick.TotalSeconds);
    ///     sut.TickIntervalMilliseconds = 500;
    ///     sut.StopAfter(2000);
    ///     await sut.StartAsync();
    ///     var durationInMilliseconds = sut.ProgressTick.TotalMilliseconds;
    /// </code>
    /// </example>
    /// Please, observe that the number of <see cref="RunningTime"/> events can vary depending on how busy the threads are.
    /// However, the intervals are self-adjusted to provide the closest number of ticks expected. 
    /// </summary>
    public class ProgressiveTimer /*: INotifyPropertyChanged*/
    {
        #region events
        /// <summary>
        /// Event is raised when <see cref="EndTime"/> time is reached
        /// </summary>
        public event EventHandler<TimeSpan?> RunCompleted;
        public event EventHandler<TimeSpan> Tick;

        #endregion events

        #region properties

        #region RunningTimeSegments

        public List<TimeSpan> RunningTimeSegments
        {
            get { return _runningTimeSegments ?? (_runningTimeSegments = new List<TimeSpan>()); }
            protected set { _runningTimeSegments = value; }
        }
        #endregion RunningTimeSegments

        #region RunningTime
        public TimeSpan TotalRunningTime
        {
            get {
                var sumOfSegments = new TimeSpan(RunningTimeSegments.Sum(s => s.Ticks));
                if (IsPaused) { return sumOfSegments; }
                else { return sumOfSegments + (DateTimeNow - StartTime); }
            }
            protected set { _totalRunningTime = value; }
        }
        #endregion RunningTime

        #region StartTime
        private DateTime _startTime;
        /// <summary>
        /// Time when <see cref="Start"/> or <see cref="StartAsync"/> was invoked.
        /// </summary>
        public DateTime StartTime
        {
            get => _startTime;
            protected set => _startTime = value;
        }
        #endregion StartTime

        #region DefaultMaxDurationMilliseconds
        /// <summary>
        /// sets the run default time to a Maximum of 20 seconds unless overriden by one of the "Stop" calls.
        /// It will be ignored if <see cref="StopAfter"/> or <see cref="StopAt"/>  is invoked or <see cref="StopAfterMilliseconds"/> is set
        /// </summary>
        private const int DefaultMaxDurationMilliseconds = 20000;
        #endregion DefaultMaxDurationMilliseconds

        #region EndTime

        private DateTime? _endTime;
        /// <summary>
        /// Time at which this Timer will stop looping and raising PropertyChanged events for the <see cref="RunningTime"/> property.
        /// This property is setup by calling <see cref="StopAt"/> or <see cref="StopAfter"/>
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                if (_endTime != null) return (DateTime)_endTime;
                if (StopAfterMilliseconds != 0) return StartTime.AddMilliseconds(StopAfterMilliseconds);
                return StartTime.AddMilliseconds(DefaultMaxDurationMilliseconds);
            }
            protected set => _endTime = value;
        }

        #endregion EndTime

        #region StopAfterMilliseconds
        private int _stopAfterMilliseconds;
        /// <summary>
        /// How long the clock should run in milliseconds.
        /// This property is set by <see cref="StopAfter"/> and is used in the calculation of <see cref="EndTime"/>
        /// </summary>
        public int StopAfterMilliseconds
        {
            get
            {
                if (_stopAfterMilliseconds <= 0) _stopAfterMilliseconds = DefaultMaxDurationMilliseconds;
                return _stopAfterMilliseconds;
            }
            set => _stopAfterMilliseconds = value;
        }
        #endregion StopAfterMilliseconds

        #region IsCancelled
        /// <summary>
        ///  This property indicates if <see cref="Cancel"/> was invoked
        /// </summary>
        public bool IsCancelled { get; protected set; }
        #endregion IsCancelled

        #region TickIntervalMilliseconds
        private const int _minimumTickIntervalMilliseconds = 200;
        private int _tickIntervalMilliseconds = _minimumTickIntervalMilliseconds;
        /// <summary>
        /// Indicates how frequently Property Changed event should be raised for the property <see cref="RunningTime"/>.
        /// Minimum Tick interval is 100 milliseconds.
        /// </summary>
        public int TickIntervalMilliseconds
        {
            get => _tickIntervalMilliseconds;
            set {
                if (value <= _minimumTickIntervalMilliseconds) value = _minimumTickIntervalMilliseconds;
                _tickIntervalMilliseconds = value;
            }
        }
        #endregion TickIntervalMilliseconds

        #region CancellationTokenFactory
        private CancellationTokenSource _cancellationTokenFactory;
        private CancellationTokenSource CancellationTokenFactory
        {
            get => _cancellationTokenFactory ?? (_cancellationTokenFactory = new CancellationTokenSource());
            set => _cancellationTokenFactory = value;
        }
        #endregion CancellationTokenFactory

        #region CancellationToken
        /// <summary>
        /// Indicates if a run was cancelled.
        /// Its state is changed by <see cref="Cancel"/> only.
        /// </summary>
        public CancellationToken CancellationToken { get; protected set; }
        #endregion CancellationToken

        #region DateTimeNowFunc
        /// <summary>
        ///     This field is used to facilitate automated tests
        ///     This func is used to freeze the Datetime for certain tests
        ///     Just as a mock type does but easier to use.
        /// </summary>
        protected Func<DateTime> DateTimeNowFunc = () => DateTime.Now;
        #endregion DateTimeNowFunc

        #region DateTimeNow
        /// <summary>
        /// This is the current datetime <see cref="DateTime.Now"/>.
        /// It gets its value from a function property named DateTimeNowFunc
        /// </summary>
        private DateTime DateTimeNow => DateTimeNowFunc();
        #endregion DateTimeNow

        #region IsRunning
        private bool _isRunning;
        /// <summary>
        /// Indicates that the timer has started (<see cref="StartTime"/> or <see cref="Start"/> invoked).
        /// It will return false when timer stopped (<see cref="EndTime"/> reached).
        /// </summary>
        public bool IsRunning
        {
            get => _isRunning;
            set => _isRunning = value;
        }

        #endregion IsRunning

        #region IsCancelled
        /// <summary>
        ///  This property indicates if <see cref="Cancel"/> was invoked
        /// </summary>
        public bool IsPaused { get; protected set; }
        #endregion IsCancelled

        
        #endregion properties

        #region methods

        /// <summary>
        /// Calls StartAsync without waiting for a response
        /// </summary>
        public void Start()
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            StartAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        /// <summary>
        /// Starts the clock in which progress is indicated by raising <see cref="INotifyPropertyChanged.PropertyChanged"/> event for the property <see cref="RunningTime"/>
        /// The event is raised as often as determined by <see cref="TickIntervalMilliseconds"/> in milliseconds.
        /// This clock will have a hard stop when elapsed time indicated by <see cref="DefaultMaxDurationMilliseconds"/> or when time in <see cref="EndTime"/> is reached.
        /// This awaitable method returns when the clock is stopped.
        /// <example>
        /// <code>
        ///     var sut = new ProgressiveTimer();
        ///     sut.PropertyChanged += (s, e) => {if (e.PropertyName == nameof(sut.ProgressTick)) Console.WriteLine(sut.ProgressTick.TotalSeconds);
        ///     sut.TickIntervalMilliseconds = 500;
        ///     sut.StopAfter(2000);
        ///     await sut.StartAsync();
        ///     var durationInMilliseconds = sut.ProgressTick.TotalMilliseconds;
        /// </code>
        /// </example>
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            StartTime = DateTimeNow;
            CancellationToken = CancellationTokenFactory.Token; //get a fresh token for this run
            IsRunning = true;
            IsCancelled = false;
            IsPaused = false;
            RunningTimeSegments.Clear();

            try
            {
                await RunClockAsync();
            }
            finally
            {
                CancellationTokenFactory.Dispose();
                CancellationTokenFactory = null;
            }

            IsRunning = false;
            EndTime = DateTimeNow;
            if (IsCancelled == false) RunCompleted?.Invoke(this, TotalRunningTime);
            else RunCompleted?.Invoke(this, null);
        }

        /// <summary>
        /// Run a timer and raise <see cref="RunningTime"/> events, every <see cref="TickIntervalMilliseconds"/>
        /// The clock and the <see cref="RunningTime"/> events are cancellable by calling <see cref="Cancel"/> 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="frequencyMilliseconds"></param>
        /// <returns></returns>
        protected async Task RunClockAsync()
        {
            var waitTimeForNextTick = TickIntervalMilliseconds;

            var endTimeReached = EndTime < DateTimeNow;
            // Run the timer while EndTime is not reached and while StopNow is not invoked
            while (endTimeReached == false && IsCancelled == false)
            {
                await Task.Delay(waitTimeForNextTick, CancellationToken).ConfigureAwait(false);
#pragma warning disable 4014
                Task.Run(() => Tick?.Invoke(this, TotalRunningTime), CancellationToken);
#pragma warning restore 4014
                waitTimeForNextTick = GetNextTickTimeMilliseconds();
                if (CancellationToken.IsCancellationRequested) break;
                if (waitTimeForNextTick <= 0) break;
            }
        }

        protected virtual int GetNextTickTimeMilliseconds()
        {
            // Time interval in Milliseconds from the StartTime to current time
            var getTimeSpanFromStartTime = new Func<int>(() => (int)(DateTimeNow - StartTime).TotalMilliseconds);
            // number of Ticks (as in ProgressTick-Tock) in previous interval
            var numberOfTicksSinceStartTime = getTimeSpanFromStartTime() / TickIntervalMilliseconds;
            // compute interval for the next ProgressTick in Milliseconds from StartTime
            var timeSpanToNextTickMilliseconds = (numberOfTicksSinceStartTime + 1) * TickIntervalMilliseconds;
            // add interval to StartTime and subtract currentTime to determine how much waiting time for the next tick
            var waitTimeForNextTickFromNow = (StartTime.AddMilliseconds(timeSpanToNextTickMilliseconds) - DateTimeNow).TotalMilliseconds;
            // if EndTime has passed then return zero wait, which means don't wait
            if (DateTimeNow >= EndTime) return 0;
            // if current time has progressed beyond next tick time then return zero wait time, which means don't wait
            if (waitTimeForNextTickFromNow < 0) return 0;
            // if next tick time exceeds EndTime return balance to EndTime
            var balanceToEndTimeMilliseconds = ((DateTime)EndTime - DateTimeNow).TotalMilliseconds;
            if (balanceToEndTimeMilliseconds < waitTimeForNextTickFromNow)
                waitTimeForNextTickFromNow = balanceToEndTimeMilliseconds;

            return (int)waitTimeForNextTickFromNow;
        }

        /// <summary>
        /// Immediately stops the clock
        /// </summary>
        public void Cancel()
        {
            CancellationTokenFactory.Cancel(false);
            IsCancelled = true;
        }

        /// <summary>
        ///     Stops the current ProgressiveTimer's run after passed milliseconds
        /// </summary>
        /// <param name="stopAfterMilliseconds"></param>
        public void StopAfter(int stopAfterMilliseconds)
        {
            if (stopAfterMilliseconds <= 0) throw new ArgumentException(nameof(stopAfterMilliseconds));
            StopAfterMilliseconds = stopAfterMilliseconds;
            _endTime = null;
        }

        /// <summary>
        ///     Stops the current ProgressiveTimer's run at a future time
        /// </summary>
        /// <param name="endTIme"></param>
        public void StopAt(DateTime endTIme)
        {
            if (endTIme < DateTimeNow) throw new ArgumentException("parameter is in the past", nameof(endTIme));
            StopAfterMilliseconds = 0;
            EndTime = endTIme;
        }

        public void Pause()
        {
            IsPaused = true;
            RunningTimeSegments.Add(DateTimeNow - StartTime);
            CancellationTokenFactory.Cancel(false);
        }

        #endregion methods

    }

}