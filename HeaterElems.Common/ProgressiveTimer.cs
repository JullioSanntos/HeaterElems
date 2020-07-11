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
    /// It also provides a series of progress notifications through <see cref="Tick"/> (as in Tick/Tock).
    /// 
    /// The timer completed notification is done through any one of two approaches:
    ///     1) By awaiting the <see cref="StartAsync"/> call to return;
    ///     2) By subscribing to the <see cref="RunCompleted"/> event.
    /// 
    /// Progress notifications are done through <see cref="Tick"/> event which
    /// identifies property <see cref="TotalRunningTime"/> as having changed.
    ///
    /// <example>
    /// In this example ProgressiveTime raises <see cref="RunCompleted"/> event after 2 seconds after
    /// it raises <see cref="TotalRunningTime"/> PropertyChange event every 1/2 second.
    /// <code>
    ///     var sut = new ProgressiveTimer();
    ///     sut.PropertyChanged += (s, e) => {if (e.PropertyName == nameof(sut.ProgressTick)) Console.WriteLine(sut.ProgressTick.TotalSeconds);
    ///     sut.TickIntervalMilliseconds = 500;
    ///     sut.StopAfter(2000);
    ///     await sut.StartAsync();
    ///     var durationInMilliseconds = sut.ProgressTick.TotalMilliseconds;
    /// </code>
    /// </example>
    /// Please, observe that the number of <see cref="TotalRunningTime"/> events can vary depending on how busy the threads are.
    /// However, the intervals are self-adjusted to provide the closest number of ticks expected. 
    /// </summary>
    public class ProgressiveTimer : IProgressiveTimer
    {
        #region events
        /// <summary>
        /// Event is raised when <see cref="EndTime"/> time is reached, unless it was cancelled by invoking <see cref="Cancel"/>
        /// This event is invoked synchronously.
        /// </summary>
        public event EventHandler<TimeSpan?> RunCompleted;
        /// <summary>
        /// Event is raised every <see cref="TickIntervalMilliseconds"/>, for as long as this Timer is "Active"
        /// This event is invoked asynchronously on another thread than the one that started this Timer.
        /// This event is cancellable by invoking <see cref="Cancel"/>
        /// All subscribers receive the <see cref="CancellationToken"/> to, cooperatively and graciously, handle a cancellation.
        /// </summary>
        public event EventHandler<Tuple<TimeSpan, CancellationToken>> Tick;

        #endregion events

        #region properties

        #region TimerState
        /// <summary>
        /// <see cref="ProgressiveTimerStateEnum"/>
        /// "Idle" is the state before this Timer is started (<see cref="Start"/> or <see cref="StartAsync"/> is invoked) or
        ///     after the <see cref="RunCompleted"/> event returns control to this Timer.
        /// "Active" Indicates that the timer has started (<see cref="StartTime"/> or <see cref="Start"/> was invoked).
        /// "Paused"  indicates that <see cref="Pause"/> was invoked
        /// "Cancelled" indicates that <see cref="Cancel"/> was invoked
        /// "Completed" means that the EndTime was reached and its scope is just for the duration of the <see cref="RunCompleted"/> event.
        /// </summary>
        public ProgressiveTimerStateEnum TimerState { get; private set; }
        #endregion TimerState

        #region DefaultMaxDurationMilliseconds
        /// <summary>
        /// sets the run default time to a Maximum of 20 seconds unless overriden by one of the "Stop" calls.
        /// It will be ignored if <see cref="StopAfter"/> or <see cref="StopAt"/>  is invoked or <see cref="StopAfterMilliseconds"/> is set
        /// </summary>
        public const int DefaultMaxDurationMilliseconds = 20000;
        #endregion DefaultMaxDurationMilliseconds

        #region RunningTimeSegments
        private List<TimeSpan> _runningTimeSegments;
        public List<TimeSpan> RunningTimeSegments
        {
            get { return _runningTimeSegments ?? (_runningTimeSegments = new List<TimeSpan>()); }
            protected set { _runningTimeSegments = value; }
        }
        #endregion RunningTimeSegments

        #region TotalRunningTime

        private TimeSpan _totalRunningTime;
        /// <summary>
        /// It returns the total time that this Timer has been running.
        /// If this Timer is running the total running time keeps increasing.
        /// If this Timer has stopped or paused, this property returns the timespan between
        /// a Start (<see cref="Start"/> or <see cref="StartAsync"/>)
        /// and a Stop (<see cref="StopAt"/> or <see cref="StopAfter"/>).
        /// If Pause was invoked (<see cref="Pause"/>, the Pause time interval is not counted by
        /// accumulating only the Running Time Segments in this property.
        /// </summary>
        public TimeSpan TotalRunningTime
        {
            get
            {
                if (_totalRunningTime.Milliseconds > 0) { return _totalRunningTime; } // to be used only on automated tests
                var sumOfSegments = new TimeSpan(RunningTimeSegments.Sum(s => s.Ticks));
                //if (IsPaused || Active == false) { return sumOfSegments; }
                if (TimerState != ProgressiveTimerStateEnum.Paused || TimerState != ProgressiveTimerStateEnum.Active) { return sumOfSegments; }
                else { return sumOfSegments + (DateTimeNow - StartTime); }
            }
            protected set { _totalRunningTime = value; } // to be used only on automated tests
        }
        #endregion TotalRunningTime

        #region StartTime

        /// <summary>
        /// Time when <see cref="Start"/> or <see cref="StartAsync"/> was invoked.
        /// </summary>
        public DateTime StartTime { get; protected set; }

        #endregion StartTime

        #region EndTime
        private DateTime? _endTime;
        /// <summary>
        /// Time at which this Timer will stop running and will stop raising <see cref="Tick"/> events.
        /// This property is setup by calling <see cref="StopAt"/> or <see cref="StopAfter"/>.
        /// This property is also adjusted on a re-start (<see cref="Start"/>) after a <see cref="Pause"/>.
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                if (_endTime != null) return (DateTime)_endTime; // This is a Calculated property, therefore, the setter should only be used for tests.
                if (StopAtDateTime != null) return (DateTime)StopAtDateTime;
                if (StartTime == DateTime.MinValue) return DateTime.MinValue;
                if (StopAfterMilliseconds > 0) return StartTime.AddMilliseconds(StopAfterMilliseconds);
                // return default maximum duration from current time if neither Stop properties has been set yet
                return DateTimeNow.AddMilliseconds(DefaultMaxDurationMilliseconds);
            }
            protected set { _endTime = value; } // this setter should only be used on Unit Tests. This is a Calculated property.
        }

        #endregion EndTime

        #region StopAtDateTime
        public DateTime? StopAtDateTime { get; private set; }
        #endregion StopAtDateTime

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
            private set { _stopAfterMilliseconds = value; }
        }
        #endregion StopAfterMilliseconds

        #region TickIntervalMilliseconds
        public const int MinimumTickIntervalMilliseconds = 200;
        private int _tickIntervalMilliseconds = MinimumTickIntervalMilliseconds;
        /// <summary>
        /// Indicates how frequently the <see cref="Tick"/> event is raised
        /// Minimum Tick interval is 100 milliseconds.
        /// </summary>
        public int TickIntervalMilliseconds
        {
            get { return _tickIntervalMilliseconds; }
            set
            {
                if (value <= MinimumTickIntervalMilliseconds) value = MinimumTickIntervalMilliseconds;
                _tickIntervalMilliseconds = value;
            }
        }
        #endregion TickIntervalMilliseconds

        #region CancellationTokenFactory
        private CancellationTokenSource _cancellationTokenFactory;
        private CancellationTokenSource CancellationTokenFactory
        {
            get { return _cancellationTokenFactory ?? (_cancellationTokenFactory = new CancellationTokenSource()); }
            set { _cancellationTokenFactory = value; }
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
        ///     This func is used to manipulate the Datetime for certain tests
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

        #endregion properties

        #region methods

        /// <summary>
        /// Calls <see cref="StartAsync"/> without waiting for a response.
        /// Please, see <see cref="StartAsync"/> documentation.
        /// <example>
        /// <code>
        ///     var sut = new ProgressiveTimer();
        ///     var isCompleted = false;;
        ///     sut.RunCompleted += (s, e) => isCompleted = true;
        ///     sut.Start(); 
        ///     sut.StopAfter(1000);
        /// </code>
        /// </example>
        /// </summary>        
        public void Start()
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            StartAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        /// <summary>
        /// 
        /// Starts a timer in which progress is indicated by raising the <see cref="Tick"/> event
        /// The event is raised as often as determined by <see cref="TickIntervalMilliseconds"/> in milliseconds.
        /// This timer will have a hard stop when elapsed time indicated by <see cref="EndTime"/> is reached.
        /// This awaitable <see cref="StartAsync"/>method returns when the Timer is stopped because <see cref="EndTime"/> was reached or
        ///     <see cref="Cancel"/> was invoked
        /// When the times stops the <see cref="RunCompleted"/> is raised
        /// <example>
        /// <code>
        ///     var sut = new ProgressiveTimer();
        ///     var isCompleted = false;;
        ///     sut.RunCompleted += (s, e) => isCompleted = true;
        ///     sut.StopAfter(1000);
        ///     await sut.StartAsync();
        /// </code>
        /// </example>
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            // Adjust EndTime, if after a Pause and StopAfterMilliseconds was set, by subtracting last run timespan from StopAfterMilliseconds
            if (TimerState == ProgressiveTimerStateEnum.Paused && StopAfterMilliseconds > 0) { StopAfterMilliseconds -= (int)RunningTimeSegments.Last().TotalMilliseconds; }
            if (TimerState != ProgressiveTimerStateEnum.Paused) { RunningTimeSegments.Clear(); }

            _endTime = null; //The property EndTime is non-nullable. Only the backing field can be reset.
            StartTime = DateTimeNow;
            CancellationToken = CancellationTokenFactory.Token; //get a fresh token for this run
            TimerState = ProgressiveTimerStateEnum.Active;

            try
            {
                await RunClockAsync();
            }
            finally
            {
                CancellationTokenFactory.Dispose();
                CancellationTokenFactory = null;
            }

            EndTime = DateTimeNow;
            RunningTimeSegments.Add(DateTimeNow - StartTime);
            if (TimerState == ProgressiveTimerStateEnum.Cancelled) { RunCompleted?.Invoke(this, null); }
            else {
                TimerState = ProgressiveTimerStateEnum.Completed;
                RunCompleted?.Invoke(this, TotalRunningTime);
            }
            
            TimerState = ProgressiveTimerStateEnum.Idle;

        }

        /// <summary>
        /// Run a timer and raise <see cref="TotalRunningTime"/> events, every <see cref="TickIntervalMilliseconds"/>
        /// This Timer's clock and the <see cref="Tick"/> events are cancellable by calling <see cref="Cancel"/> 
        /// </summary>
        /// <returns></returns>
        protected async Task RunClockAsync()
        {
            var waitTimeForNextTick = TickIntervalMilliseconds;

            var endTimeReached = new Func<bool>(() => EndTime < DateTimeNow);
            // Run the timer while EndTime is not reached and while StopNow is not invoked
            while (endTimeReached() == false && TimerState != ProgressiveTimerStateEnum.Cancelled)
            {
                await Task.Delay(waitTimeForNextTick, CancellationToken).ConfigureAwait(false);
#pragma warning disable 4014
                // by design this is a fire-and-forget call. This timer shouldn't be blocked by clients code
                Task.Run(() => Tick?.Invoke(this, new Tuple<TimeSpan, CancellationToken>(TotalRunningTime, CancellationToken)), CancellationToken);
#pragma warning restore 4014
                if (CancellationToken.IsCancellationRequested) { break; }
                waitTimeForNextTick = GetNextTickTimeMilliseconds();
                if (waitTimeForNextTick <= 0) { waitTimeForNextTick = TickIntervalMilliseconds; }
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
            if (DateTimeNow >= EndTime) { return 0; }
            // if current time has progressed beyond next tick time then return zero wait time, which means don't wait
            if (waitTimeForNextTickFromNow < 0) { return 0; }
            // if next tick time exceeds EndTime return balance to EndTime
            var balanceToEndTimeMilliseconds = ((DateTime)EndTime - DateTimeNow).TotalMilliseconds;
            if (balanceToEndTimeMilliseconds < waitTimeForNextTickFromNow)
            { waitTimeForNextTickFromNow = balanceToEndTimeMilliseconds; }

            return (int)waitTimeForNextTickFromNow;
        }

        /// <summary>
        /// Immediately stops the clock
        /// </summary>
        public void Cancel()
        {
            CancellationTokenFactory.Cancel(false);
            TimerState = ProgressiveTimerStateEnum.Cancelled;
        }

        /// <summary>
        ///     Stops the current ProgressiveTimer's run after passed milliseconds
        /// </summary>
        /// <param name="stopAfterMilliseconds"></param>
        public void StopAfter(int stopAfterMilliseconds)
        {
            if (stopAfterMilliseconds <= 0) { throw new ArgumentException(nameof(stopAfterMilliseconds)); }
            StopAfterMilliseconds = stopAfterMilliseconds;
            StopAtDateTime = null;
        }

        /// <summary>
        ///     Stops the current ProgressiveTimer's run at a future time
        /// </summary>
        /// <param name="endTIme"></param>
        public void StopAt(DateTime endTIme)
        {
            if (endTIme < DateTimeNow) { throw new ArgumentException("parameter is in the past", nameof(endTIme)); }
            StopAtDateTime = endTIme;
            StopAfterMilliseconds = 0;
        }

        /// <summary>
        /// Pause the time count and stop the <see cref="Tick"/> event from being raised.
        /// Use <see cref="Start"/> or <see cref="StartAsync"/> to resume the time count.
        /// </summary>
        public void Pause()
        {
            TimerState = ProgressiveTimerStateEnum.Paused;
            RunningTimeSegments.Add(DateTimeNow - StartTime);
            CancellationTokenFactory.Cancel(false);
        }

        #endregion methods

    }

    public enum ProgressiveTimerStateEnum
    {
        Idle,
        Active,
        Paused,
        Completed,
        Cancelled
    }

}