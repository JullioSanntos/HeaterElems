using HeaterElems.Common.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;

namespace HeaterElems.ViewModels
{
    public class MainViewModel : SetPropertyBase
    {
        // In Testing branch
        #region HeatingWatch
        private StopWatch _heatingWatch;
        public StopWatch HeatingWatch {
            get => _heatingWatch ?? (_heatingWatch = new StopWatch());
            set => SetProperty(ref _heatingWatch, value);
        }
        #endregion HeatingWatch

        #region StartCommand
        private RelayCommand _startCommand;
        public RelayCommand StartCommand {
            get { return _startCommand ?? _startCommand ?? (_startCommand = new RelayCommand((v) => StartWatch(null))); }
            set { SetProperty(ref _startCommand, value); }
        }
        #endregion StartCommand

        #region CountDownValue
        private double _duration = 3;
        public double Duration {
            get => _duration;
            set => SetProperty(ref _duration, value);
        }
        #endregion CountDownValue

        public void StartWatch(object _)
        {
            IsCompleted = false;
            HeatingWatch.Completed += (s, a) => IsCompleted = true;
            HeatingWatch.Start().ConfigureAwait(false);
            HeatingWatch.StopAfter(Duration * 1000);
        }

        #region IsCompleted
        private bool _isCompleted;
        public bool IsCompleted {
            get { return _isCompleted; }
            set { SetProperty(ref _isCompleted, value); }
        }
        #endregion IsCompleted
    }
}
