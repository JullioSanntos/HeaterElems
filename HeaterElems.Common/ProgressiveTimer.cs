using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

//namespace Common.Utilities
namespace HeaterElems.Common
{
    /// <summary>
    /// Provides a way for clients to be notified when an specified time was reached.
    /// It also provides a series of progress notifications through <see cref="ProgressTick"/> (as in Tick/Tock).
    /// 
    /// The timer completed notification is done through any one of two approaches:
    ///     1) By awaiting the <see cref="StartAsync"/> call to be returned;
    ///     2) By subscribing to the <see cref="RunCompleted"/> event.
    /// 
    /// Progress notifications is done through <see cref="INotifyPropertyChanged.PropertyChanged"/> events which
    /// identifies property <see cref="ProgressTick"/> as having changed.
    ///
    /// <example>
    /// In this example ProgressiveTime raises <see cref="RunCompleted"/> event after 2 seconds after
    /// it raises <see cref="ProgressTick"/> PropertyChange event every 1/2 second.
    /// <code>
    ///     var sut = new ProgressiveTimer();
    ///     sut.PropertyChanged += (s, e) => {if (e.PropertyName == nameof(sut.ProgressTick)) Console.WriteLine(sut.ProgressTick.TotalSeconds);
    ///     sut.TickIntervalMilliseconds = 500;
    ///     sut.StopAfter(2000);
    ///     await sut.StartAsync();
    ///     var durationInMilliseconds = sut.ProgressTick.TotalMilliseconds;
    /// </code>
    /// </example>
    /// Please, observe that the number of <see cref="ProgressTick"/> events can vary depending on how busy the threads are.
    /// However, the intervals are self-adjusted to provide the closest number of ticks expected. 
    /// </summary>
    public class ProgressiveTimer : INotifyPropertyChanged
    {
        #region events
        /// <summary>
        /// Event is raised when <see cref="EndTime"/> time is reached
        /// </summary>
        public event EventHandler<DateTime?> RunCompleted;

        #endregion events

        #region properties

        #region ProgressTick
        /// <summary>
        /// Elapsed time from <see cref="StartTime" /> to the time this property is interrogated
        /// </summary>
        public TimeSpan ProgressTick => DateTimeNow - (DateTime)StartTime;
        #endregion ProgressTick

        #region StartTime
        private DateTime? _startTime;
        /// <summary>
        /// Time when <see cref="Start"/> or <see cref="StartAsync"/> was invoked.
        /// </summary>
        public DateTime StartTime
        {
            get => (DateTime)(_startTime ?? (_startTime = DateTimeNow));
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
        /// Time at which this Timer will stop looping and raising PropertyChanged events for the <see cref="ProgressTick"/> property.
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
            set => SetProperty(ref _stopAfterMilliseconds, value);
        }
        #endregion StopAfterMilliseconds

        #region WasStopped

        /// <summary>
        ///  This property indicates if <see cref="StopNow"/> was invoked
        /// </summary>
        public bool WasStopped { get; protected set; }

        #endregion HasStopped

        #region TickIntervalMilliseconds
        private const int _minimumTickIntervalMilliseconds = 100;
        private int _tickIntervalMilliseconds = _minimumTickIntervalMilliseconds;
        /// <summary>
        /// Indicates how frequently Property Changed event should be raised for the property <see cref="ProgressTick"/>.
        /// Minimum Tick interval is 100 milliseconds.
        /// </summary>
        public int TickIntervalMilliseconds
        {
            get => _tickIntervalMilliseconds;
            set {
                if (value <= _minimumTickIntervalMilliseconds) value = _minimumTickIntervalMilliseconds;
                SetProperty(ref _tickIntervalMilliseconds, value);
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
        /// Its state is changed by <see cref="StopNow"/> only.
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
            set => SetProperty(ref _isRunning, value);
        }

        #endregion IsRunning

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
        /// Starts the clock in which progress is indicated by raising <see cref="INotifyPropertyChanged.PropertyChanged"/> event for the property <see cref="ProgressTick"/>
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

            await RunClockAsync((DateTime)StartTime, TickIntervalMilliseconds);

            IsRunning = false;
            EndTime = DateTimeNow;
            RunCompleted?.Invoke(this, EndTime);
        }


        /// <summary>
        /// Run a timer and raise <see cref="ProgressTick"/> events, every <see cref="TickIntervalMilliseconds"/>
        /// The clock and the <see cref="ProgressTick"/> events are cancellable by calling <see cref="StopNow"/> 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="frequencyMilliseconds"></param>
        /// <returns></returns>
        protected async Task RunClockAsync(DateTime startTime, int frequencyMilliseconds)
        {
            var waitTimeForNextTick = frequencyMilliseconds;

            var endTimeReached = EndTime < DateTimeNow;
            // Run the timer while EndTime is not reached and while StopNow is not invoked
            while (endTimeReached == false && WasStopped == false)
            {
                await Task.Delay(waitTimeForNextTick, CancellationToken);
#pragma warning disable 4014
                Task.Run(() => RaisePropertyChanged(nameof(ProgressTick)), CancellationToken);
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
        public void StopNow()
        {
            CancellationTokenFactory.Cancel(false);
            WasStopped = true;
            RunCompleted?.Invoke(this, DateTimeNow);
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

        #endregion methods

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [DebuggerStepThrough]
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [DebuggerStepThrough]
        public void SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(value, backingField)) return;

            backingField = value;

            RaisePropertyChanged(propertyName);
        }
        #endregion INotifyPropertyChanged

        //FOR MY RECORDS: 
        //  
        //
        // WHY I DIDN'T USE System.Timers.Timer type: https://docs.microsoft.com/en-us/dotnet/api/system.timers.timer?view=netframework-4.8
        //
        // This type implements the IDisposable interface... 
        // The Timer component catches and suppresses all exceptions thrown by event handlers for the Elapsed event.
        // This behavior is subject to change in future releases of the .NET Framework. Note, however,
        //      that this is not true of event handlers that execute asynchronously and include the await operator (in C#)
        //      Exceptions thrown in these event handlers are propagated back to the calling thread, ...
        // If the SynchronizingObject property is null, the Elapsed event is raised on a ThreadPool thread.
        // If processing of the Elapsed event lasts longer than Interval, the event might be raised again on another ThreadPool thread.
        // In this situation, the event handler should be re-entrant.
        // Even if SynchronizingObject is not null, Elapsed events can occur after the Dispose or Stop method has been called or
        //      after the Enabled property has been set to false, because the signal to raise the Elapsed event is always queued for execution on a thread pool thread.
        //      One way to resolve this race condition is to set a flag that tells the event handler for the Elapsed event to ignore subsequent events.
        //
        //
        // WHY I DIDN'T USE the System.Diagnostics.StopWatch https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch?view=netframework-4.8
        // 
        // Stopwatch doesn't show progress
        // Stopwatch doesn't have a count down. It is still up to the client to determine when to stop the watch.
        // Stopwatch doesn't raise events.
        // Stopwatch can't be called asynchronously (async/await).
        // Stopwatch doesn't show many other properties of interest for the consumer  
        // It is not cancellable
        //  
        // WHY I DIDN'T USE Common.Utilities.HighPrecisionTimer
        //  
        // This type implements the IDisposable interface... 
        // It can't be called asynchronously (async/await).
        // It is not cancellable
        //  Despite of the name, there are more precise Timers like the .net based Stopwatch (based on hardware),
        //      but this story, "Heater Sit Time", doesn't need much precision
        //  Relies on old Native Window API's that may be deprecated sooner than .NET framework
        //  Desktop operating system (such as windows) are not real-time operating system, which means, you can't expect
        //      full accuracy because you can't force the scheduler to trigger your code in the exact millisecond you want
        //  
        //  
        // WHY I DIDN'T USE the Common.Utilities.Wrappers.StopWatchWrapper (wrapper for testing purposes):
        // WHY I DIDN"T USE Common.Utilities.Wrappers.TimerWrapper (wrapper for testing purposes):
        //  
        //  

    }

}