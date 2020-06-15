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
    public class CountDownWatch : SetPropertyBase
    {
        public TimeSpan CountDownValue { get; private set; }

        #region Elapsed
        // ReSharper disable once PossibleLossOfFraction
        public double Elapsed {
            get {
                var elapsedTime = EndTime - DateTime.Now;
                var elapsedInMilliSeconds = (elapsedTime.Seconds + (elapsedTime.Milliseconds) / (double) 1000);
                if (elapsedInMilliSeconds <= 0) return 0;

                return Math.Round(elapsedInMilliSeconds, 1);
            }
        }
        #endregion Elapsed

        #region StartTime
        private DateTime? _startTime;
        public DateTime? StarTime => _startTime ?? (_startTime = DateTime.Now);
        #endregion StartTime

        #region EndTime
        private DateTime? _endTime;
        public DateTime EndTime {
            get {
                if (_endTime == null) _endTime = StarTime + CountDownValue;
                return (DateTime)_endTime;
            }
        }
        #endregion EndTime

        public int RefreshIntervalInMiliSeconds => 100;

        public EventHandler CountDownCompleted;

        public async Task Start(TimeSpan countDownValue)
        {
            if (CountDownValue == TimeSpan.MinValue) 
                throw new ArgumentException(nameof(countDownValue));
            
            _startTime = _endTime = null;

            CountDownValue = countDownValue;
            
            while (DateTime.Now < EndTime) {
                RaisePropertyChanged(nameof(Elapsed));
                await Task.Delay(RefreshIntervalInMiliSeconds).ConfigureAwait(false);
            }

            CountDownCompleted?.Invoke(this, new EventArgs());
            RaisePropertyChanged(nameof(Elapsed));
        }
    }
}
