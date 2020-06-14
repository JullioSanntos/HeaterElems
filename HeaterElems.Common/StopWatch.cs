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
                else return (DateTime) (_endTime ?? (_endTime = StartTime + TimeSpan.FromMilliseconds(SetDurationInMilliSeconds)));
            }
            private set => _endTime = value;
        }
        #endregion EndTime


        #region SetDurationInMilliSeconds
        private int _setDurationInMilliSeconds;
        public int SetDurationInMilliSeconds {
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
        private int _refreshFrequencyInMilliseconds = 100;
        public int RefreshFrequencyInMilliseconds {
            get { return _refreshFrequencyInMilliseconds; }
            set { SetProperty(ref _refreshFrequencyInMilliseconds, value); }
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
        private CancellationToken _cancellationToken;
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
        public event EventHandler Completed;
        #endregion events

        #region methods
        public async Task StartAsync() {
            //if (EndTime < DateTimeNow || _startTime < DateTimeNow)
            StartTime = DateTimeNow;
            _cancellationToken = CancellationTokenFactory.Token; //get a fresh token for this run

            await RunClockAsync();

            Completed?.Invoke(this, new EventArgs());
        }

        protected internal async Task RunClockAsync() {
            
            while (DateTimeNow <= EndTime && WasStopped == false) {
                await Task.Delay(RefreshFrequencyInMilliseconds, _cancellationToken);
                RaisePropertyChanged(nameof(RunDuration));
                if (_cancellationToken.IsCancellationRequested) break;

                ////Adjust last loop's stoptime if refreshRate doesn't happen soon enough
                //stopTime = GetAdjustedStopTime(EndTime, RefreshFrequencyInMilliseconds);
            }

        }

        protected internal DateTime GetAdjustedStopTime(DateTime endTime, int refreshFrequencyInMilliseconds) {
            var balanceOfRunTime = endTime - DateTimeNow;
            if (balanceOfRunTime.Milliseconds < refreshFrequencyInMilliseconds) return DateTimeNow.AddMilliseconds(refreshFrequencyInMilliseconds);
            else return endTime;
        }

        public void Stop() {
            CancellationTokenFactory.Cancel(false);
            WasStopped = true;
        }

        public void StopAfter(int milliSeconds) {
            if (milliSeconds <= 0) throw new ArgumentException(nameof(milliSeconds));
            SetDurationInMilliSeconds = milliSeconds;
        }

        public void StopAt(DateTime stopTime) {
            if (stopTime < DateTimeNow) throw new ArgumentException("parameter is in the past", nameof(stopTime));
            EndTime = stopTime;
        }

        public void Cancel()
        {
            CancellationTokenFactory.Cancel();
            //CancellationTokenFactory = null; //Reset to be lazy instantiated
        }
        #endregion methods

    }
}
