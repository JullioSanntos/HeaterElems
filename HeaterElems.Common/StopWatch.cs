using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using HeaterElems.Common.Annotations;

namespace HeaterElems.Common
{
    public class StopWatch : SetPropertyBase
    {
        public TimeSpan ElapsedTime => DateTime.Now - StarTime;

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
        public DateTime StarTime => (DateTime)(_startTime ?? (_startTime = DateTime.Now));
        #endregion StartTime

        #region EndTime
        public DateTime EndTime {
            get => StarTime + TimeSpan.FromMilliseconds(Duration);
        }
        #endregion EndTime

        #region Duration
        private double _duration;
        public double Duration {
            get { return _duration; }
            set { SetProperty(ref _duration, value); }
        }
        #endregion Duration

        #region HasStopped
        private bool _hasStopped;
        public bool HasStopped {
            get => _hasStopped;
            private set => SetProperty(ref _hasStopped, value);
        }
        #endregion HasStopped

        public int RefreshRateInMiliSeconds => 100;

        public event EventHandler CountDownCompleted;

        public async Task Start()
        {
            //if (EndTime < DateTime.Now || _startTime < DateTime.Now)
            _startTime = null; //Reset to be lazy instantiated


            while (DateTime.Now <= EndTime || HasStopped) {
                RaisePropertyChanged(nameof(ElapsedTotalSeconds));
                await Task.Delay(RefreshRateInMiliSeconds);
            }

            CountDownCompleted?.Invoke(this, new EventArgs());
            ElapsedTotalSeconds = Math.Round(ElapsedTotalSeconds, 0);
            RaisePropertyChanged(nameof(ElapsedTotalSeconds));
        }

        public void Stop()
        {
            HasStopped = true;
        }

        public void StopAfter(double durationInMilliSeconds)
        {
            if (durationInMilliSeconds <= 0) throw new ArgumentException(nameof(durationInMilliSeconds));
            Duration = durationInMilliSeconds;
        }
    }
}
