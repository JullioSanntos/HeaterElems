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
        public TimeSpan ElapsedTime => DateTime.Now - StarTime;
        #endregion ElapsedTime

        #region ElapsedTotalSeconds
        private double _elapsedTotalSeconds;
        // ReSharper disable once PossibleLossOfFraction
        public double ElapsedTotalSeconds {
            get => Math.Round(ElapsedTime.TotalSeconds, 1);
            private set => _elapsedTotalSeconds = value;
        }
        #endregion ElapsedTotalSeconds

        #region StartTime
        private DateTime? _startTime;
        private DateTime StarTime => (DateTime)(_startTime ?? (_startTime = DateTime.Now));
        #endregion StartTime

        #region EndTime
        private DateTime EndTime {
            get => StarTime + TimeSpan.FromMilliseconds(Duration);
        }
        #endregion EndTime

        #region Duration
        private int _duration;
        public int Duration {
            get { return _duration; }
            set { SetProperty(ref _duration, value); }
        }
        #endregion Duration

        #region HasStopped
        private bool _stopped;
        public bool Stopped {
            get => _stopped;
            private set => SetProperty(ref _stopped, value);
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
        public CancellationToken CancellationToken;
        #endregion CancellationToken

        #endregion properties


        public event EventHandler Completed;

        public async Task Start()
        {
            //if (EndTime < DateTime.Now || _startTime < DateTime.Now)
            _startTime = null; //Reset to be lazy instantiated
            CancellationToken = CancellationTokenFactory.Token; //get a fresh token for this run

            await AwaitStopOrCancel();

            Completed?.Invoke(this, new EventArgs());
            ElapsedTotalSeconds = Math.Round(ElapsedTotalSeconds, 0);
        }

        private async Task AwaitStopOrCancel()
        {
            while (DateTime.Now <= EndTime || Stopped)
            {
                await Task.Delay(RefreshRateInMilliseconds, CancellationToken);
                RaisePropertyChanged(nameof(ElapsedTotalSeconds));
                if (CancellationToken.IsCancellationRequested) break;
            }
        }

        public void Stop()
        {
            Stopped = true;
        }

        public void StopAfter(int durationInMilliSeconds)
        {
            if (durationInMilliSeconds <= 0) throw new ArgumentException(nameof(durationInMilliSeconds));
            Duration = durationInMilliSeconds;
        }

        public void Cancel()
        {
            CancellationTokenFactory.Cancel();
            //CancellationTokenFactory = null; //Reset to be lazy instantiated
        }
    }
}
