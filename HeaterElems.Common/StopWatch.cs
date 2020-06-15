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
    public class StopWatch : SetPropertyBase
    {
        #region properties

        #region RunDuration
        public TimeSpan RunDuration {
            get {
                if (StartTime == null) return TimeSpan.Zero;
                else return DateTimeNow - (DateTime)StartTime;
            }
        }
        #endregion RunDuration

        #region StartTime
        private DateTime? _startTime;
        public DateTime? StartTime {
            get => _startTime;
            set => _startTime = value;
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

        #region RefreshFrequencyInMilliseconds
        private int _frequencyMilliseconds = 100;
        public int FrequencyMilliseconds {
            get { return _frequencyMilliseconds; }
            set { SetProperty(ref _frequencyMilliseconds, value); }
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

            await RunClockAsync();

            RunCompleted?.Invoke(this, new EventArgs());
        }

        protected internal async Task RunClockAsync() {
            
            while (DateTimeNow <= EndTime && WasStopped == false) {
                await Task.Delay(FrequencyMilliseconds, CancellationToken);
                RaisePropertyChanged(nameof(RunDuration));
                if (CancellationToken.IsCancellationRequested) break;

                ////Adjust last loop's stoptime if refreshRate doesn't happen soon enough
                //stopTime = GetAdjustedStopTime(EndTime, RefreshFrequencyInMilliseconds);
            }

        }

        protected internal DateTime GetAdjustedStopTime(DateTime endTime, int refreshFrequencyInMilliseconds) {
            var balanceOfRunTime = endTime - DateTimeNow;
            if (balanceOfRunTime.Milliseconds < refreshFrequencyInMilliseconds) return DateTimeNow.AddMilliseconds(refreshFrequencyInMilliseconds);
            else return endTime;
        }

        /// <summary>
        /// Stops the current StopWatch's run immediately in an ordered manner without an exception 
        /// </summary>
        public void Stop() {
            CancellationTokenFactory.Cancel(false);
            WasStopped = true;
            RunCompleted?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Stops the current StopWatch's run after passed milliseconds
        /// </summary>
        /// <param name="milliSeconds"></param>
        public void StopAfter(int milliSeconds) {
            if (milliSeconds <= 0) throw new ArgumentException(nameof(milliSeconds));
            SetDurationMilliSeconds = milliSeconds;
        }

        /// <summary>
        /// SStops the current StopWatch's run at a future time
        /// </summary>
        /// <param name="stopTime"></param>
        public void StopAt(DateTime stopTime) {
            if (stopTime < DateTimeNow) throw new ArgumentException("parameter is in the past", nameof(stopTime));
            EndTime = stopTime;
        }

        ///// <summary>
        ///// Cancel the StopWatch Run immediately and raises an exception
        ///// </summary>
        //public void Cancel()
        //{
        //    CancellationTokenFactory.Cancel(true);
        //}
        #endregion methods

    }
}
