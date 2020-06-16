using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using HeaterElems.Common.Annotations;

namespace HeaterElems.Common
{

    public class ProgressiveTimer : SetPropertyBase
    {
        #region properties

        #region RunProgress
        public TimeSpan RunProgress {
            get {
                if (StartTime == null) return TimeSpan.Zero;
                else return DateTimeNow - (DateTime)StartTime;
            }
        }
        #endregion RunProgress

        #region StartTime
        private DateTime? _startTime;
        public DateTime? StartTime {
            get => _startTime;
            internal set => _startTime = value;
        }
        #endregion StartTime

        #region EndTime
        private DateTime? _endTime;
        public DateTime? EndTime {
            get {
                if (StartTime == null) return null;
                else return (DateTime) (_endTime ?? (_endTime = StartTime + TimeSpan.FromMilliseconds(SetDurationMilliSeconds)));
            }
            private set => _endTime = value;
        }
        #endregion EndTime

        #region SetDurationInMilliSeconds
        private int _setDurationInMilliSeconds;
        public int SetDurationMilliSeconds {
            get {
                if (_setDurationInMilliSeconds <= 0) _setDurationInMilliSeconds = DefaultMaxDuration;
                return _setDurationInMilliSeconds;
            }
            set => SetProperty(ref _setDurationInMilliSeconds, value);
        }
        #endregion SetDurationInMilliSeconds

        #region WasStopped
        private bool _wasStopped;
        private bool WasStopped {
            get => _wasStopped;
            set => SetProperty(ref _wasStopped, value);
        }
        #endregion HasStopped

        #region TickFrequencyMilliseconds
        private int _tickFrequencyMilliseconds = 100;
        public int TickFrequencyMilliseconds {
            get { return _tickFrequencyMilliseconds; }
            set { SetProperty(ref _tickFrequencyMilliseconds, value); }
        }
        #endregion RefreshRateInMilliseconds

        #region CancellationTokenFactory
        private CancellationTokenSource _cancellationTokenFactory;
        private CancellationTokenSource CancellationTokenFactory {
            get { return _cancellationTokenFactory ?? (_cancellationTokenFactory = new CancellationTokenSource()); }
            set { SetProperty(ref _cancellationTokenFactory, value); }
        }
        #endregion CancellationTokenFactory

        #region CancellationToken
        public CancellationToken CancellationToken { get; private set; }
        #endregion CancellationToken

        #region DateTimeNow
        /// <summary>
        /// This field is used to facilitate automate tests
        /// This func is used to freeze the Datetime for certain tests
        /// </summary>
        protected internal Func<DateTime> DateTimeNowFunc = () => DateTime.Now;
        private DateTime DateTimeNow => DateTimeNowFunc();
        #endregion DateTimeNow

        #region MaxRunTime
        /// <summary>
        /// default Maximum Run Time of 20 seconds unless overriden by one of the "Stop" calls
        /// </summary>
        private const int DefaultMaxDuration = 20000; 
        #endregion MaxRunTime

        #endregion properties

        #region events
        public event EventHandler RunCompleted;
        #endregion events

        #region methods
        public void Start() {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            StartAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public async Task StartAsync() {
            //if (EndTime < DateTimeNow || _startTime < DateTimeNow)
            StartTime = DateTimeNow;
            CancellationToken = CancellationTokenFactory.Token; //get a fresh token for this run

            await RunClockAsync((DateTime)StartTime, TickFrequencyMilliseconds);

            RunCompleted?.Invoke(this, new EventArgs());
        }

        protected internal async Task RunClockAsync(DateTime startTime, int frequencyMilliseconds)
        {
            var waitTimeForNextTick = frequencyMilliseconds;
            while (DateTimeNow <= EndTime && WasStopped == false) {
                await Task.Delay(waitTimeForNextTick, CancellationToken);
                waitTimeForNextTick = GetNextTickTimeMilliseconds(startTime, frequencyMilliseconds);
                RaisePropertyChanged(nameof(RunProgress));
                if (CancellationToken.IsCancellationRequested) break;
            }
        }

        protected internal int GetNextTickTimeMilliseconds(DateTime startTime, int frequencyMilliseconds)
        {
            var timeSpanFromStartTime = (DateTimeNow - startTime).TotalMilliseconds;
            var numberOfTicksSinceStartTime = ((int)timeSpanFromStartTime / frequencyMilliseconds);
            var nextTickTimeSpanFromStartTime = (numberOfTicksSinceStartTime + 1) * frequencyMilliseconds;
            var waitTimeForNextTickFromNow = nextTickTimeSpanFromStartTime - (DateTimeNow - startTime).TotalMilliseconds;  //to be used with or added to DateTimeNow

            return (int)waitTimeForNextTickFromNow;
        }

        protected internal DateTime GetAdjustedStopTime(DateTime endTime, int refreshFrequencyInMilliseconds) {
            var balanceOfRunTime = endTime - DateTimeNow;
            if (balanceOfRunTime.Milliseconds < refreshFrequencyInMilliseconds) return DateTimeNow.AddMilliseconds(refreshFrequencyInMilliseconds);
            else return endTime;
        }

        /// <summary>
        /// Stops the current ProgressiveTimer's run immediately in an ordered manner without an exception 
        /// </summary>
        public void Stop() {
            CancellationTokenFactory.Cancel(false);
            WasStopped = true;
            RunCompleted?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Stops the current ProgressiveTimer's run after passed milliseconds
        /// </summary>
        /// <param name="milliSeconds"></param>
        public void StopAfter(int milliSeconds) {
            if (milliSeconds <= 0) throw new ArgumentException(nameof(milliSeconds));
            SetDurationMilliSeconds = milliSeconds;
        }

        /// <summary>
        /// SStops the current ProgressiveTimer's run at a future time
        /// </summary>
        /// <param name="stopTime"></param>
        public void StopAt(DateTime stopTime) {
            if (stopTime < DateTimeNow) throw new ArgumentException("parameter is in the past", nameof(stopTime));
            EndTime = stopTime;
        }

        ///// <summary>
        ///// Cancel the ProgressiveTimer Run immediately and raises an exception
        ///// </summary>
        //public void Cancel()
        //{
        //    CancellationTokenFactory.Cancel(true);
        //}
        #endregion methods

        //FOR MY RECORDS: 
        //  
        //
        // WHY I DIDN'T USE System.Timers.Timer type: https://docs.microsoft.com/en-us/dotnet/api/system.timers.timer?view=netframework-4.8
        //
        // This type implements the IDisposable interface... 
        // The Timer component catches and suppresses all exceptions thrown by event handlers for the Elapsed event.
        // This behavior is subject to change in future releases of the .NET Framework. Note, however,
        //      that this is not true of event handlers that execute asynchronously and include the await operator (in C#) or the Await operator (in Visual Basic).
        //      Exceptions thrown in these event handlers are propagated back to the calling thread, ...
        // If the SynchronizingObject property is null, the Elapsed event is raised on a ThreadPool thread.
        // If processing of the Elapsed event lasts longer than Interval, the event might be raised again on another ThreadPool thread.
        // In this situation, the event handler should be re-entrant.
        // Even if SynchronizingObject is not null, Elapsed events can occur after the Dispose or Stop method has been called or
        //      after the Enabled property has been set to false, because the signal to raise the Elapsed event is always queued for execution on a thread pool thread.
        //      One way to resolve this race condition is to set a flag that tells the event handler for the Elapsed event to ignore subsequent events.
        //
        //
        // WHY I CREATED A WRAPPER FOR The System.Diagnostics.ProgressiveTimer using Common.Utilities.Wrappers.StopWatchWrapper (wrapper for testing purposes):
        // 
        // Stopwatch doesn't show progress
        // Stopwatch doesn't have a count down.
        // Stopwatch doesn't raise events.
        // Stopwatch can't be called asynchronously (async/await).
        // Stopwatch doesn't show many other properties of interest for the consumer  
        //  
        // WHY I DIDN"T USE Common.Utilities.HighPrecisionTimer
        //  
        // This type implements the IDisposable interface... 
        //  Despite of the name, there are more precise Timers like Stopwatch (depending on hardware),
        //      but this story, "Heater Sit Time", doesn't need that
        //  Relies on old Native Window API's that may be deprecated sooner than .NET framework
        //  Desktop operating system (such as windows) are not real-time operating system, which means, you can't expect
        //      full accuracy and you can't force the scheduler to trigger your code in the exact millisecond you want
        //  
        //  
        // WHY I DIDN"T USE Common.Utilities.Wrappers.TimerWrapper
        //  
        //  It is just a wrapper for testing purposes (I believe).
        //  

        // 
        // 






    }
}
