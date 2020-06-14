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

        #region ElapsedTime
        public TimeSpan RunDuration => DateTimeNow - StarTime;
        #endregion ElapsedTime

        #region RunDurationInSeconds
        private double _elapsedTotalSeconds;
        // ReSharper disable once PossibleLossOfFraction
        public double RunDurationInSeconds {
            get => Math.Round(RunDuration.TotalSeconds, 1);
            private set => _elapsedTotalSeconds = value;
        }
        #endregion ElapsedTotalSeconds

        #region StartTime
        private DateTime? _startTime;
        public DateTime StarTime => (DateTime)(_startTime ?? (_startTime = DateTimeNow));
        #endregion StartTime

        #region EndTime
        private DateTime? _endTime;
        public DateTime EndTime {
            get => (DateTime) (_endTime ?? (_endTime = StarTime + TimeSpan.FromMilliseconds(SetDuration)));
            private set => _endTime = value;
        }
        #endregion EndTime

        #region SetDuration
        public int SetDuration { get; private set; }
        #endregion ClockRun

        #region WasStopped
        private bool _stopped;
        private bool WasStopped {
            get => _stopped;
            set => SetProperty(ref _stopped, value);
        }
        #endregion HasStopped

        #region RefreshRateInMilliseconds
        private int _refreshRateInMilliseconds = 100;
        public int RefreshRateInMilliseconds {
            get { return _refreshRateInMilliseconds; }
            set { SetProperty(ref _refreshRateInMilliseconds, value); }
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

        //#region MaxRunTime
        //private const int MaxRunTime = 10000;
        //#endregion MaxRunTime

        #endregion properties


        public event EventHandler Completed;

        public async Task StartAsync() {
            //if (EndTime < DateTimeNow || _startTime < DateTimeNow)
            _startTime = null; //Reset to be lazy instantiated
            _cancellationToken = CancellationTokenFactory.Token; //get a fresh token for this run

            await RunClockAsync();

            Completed?.Invoke(this, new EventArgs());
            RunDurationInSeconds = Math.Round(RunDurationInSeconds, 0);
        }

        protected internal async Task RunClockAsync() {
            var stopTime = EndTime;
            while (DateTimeNow <= stopTime || WasStopped)
            {
                await Task.Delay(RefreshRateInMilliseconds, _cancellationToken);
                RaisePropertyChanged(nameof(RunDurationInSeconds));
                if (_cancellationToken.IsCancellationRequested) break;

                ////Adjust last loop's stoptime if refreshRate doesn't happen soon enough
                //stopTime = GetAdjustedStopTime(EndTime, RefreshRateInMilliseconds);
            }
        }

        protected internal DateTime GetAdjustedStopTime(DateTime endTime, int refreshRateInMilliseconds) {
            var balanceOfRunTime = endTime - DateTimeNow;
            if (balanceOfRunTime.Milliseconds < refreshRateInMilliseconds) return DateTimeNow.AddMilliseconds(refreshRateInMilliseconds);
            else return endTime;
        }

        public void ImmediateStop() {
            CancellationTokenFactory.Cancel();
            WasStopped = true;
        }

        public void StopAfter(int milliSeconds) {
            if (milliSeconds <= 0) throw new ArgumentException(nameof(milliSeconds));
            SetDuration = milliSeconds;
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
    }
}
