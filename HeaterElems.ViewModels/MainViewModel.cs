﻿using HeaterElems.Common.Annotations;
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
        private ProgressiveTimer _heatingWatch;
        public ProgressiveTimer HeatingWatch {
            get { return _heatingWatch ?? (_heatingWatch = new ProgressiveTimer()); }
            set { SetProperty(ref _heatingWatch, value); }
        }
        #endregion HeatingWatch

        #region StartCommand
        private RelayCommand _startCommand;
        public RelayCommand StartCommand {
            get { return _startCommand ?? _startCommand ?? (_startCommand = new RelayCommand((v) => StartWatch(null))); }
            set { SetProperty(ref _startCommand, value); }
        }
        #endregion StartCommand

        #region RunDuration
        public double RunDuration => Math.Round(HeatingWatch.TotalRunningTime.TotalMilliseconds / 1000, 1);
        #endregion Durantion


        #region CountDownValue
        private int _setDuration = 3;
        public int SetDuration {
            get { return _setDuration; }
            set { SetProperty(ref _setDuration, value); }
        }
        #endregion CountDownValue

        public void StartWatch(object _) {
            IsCompleted = false;
            HeatingWatch.RunCompleted += (s, a) => IsCompleted = true;
            HeatingWatch.Tick += (s, e) => {
                    RaisePropertyChanged(nameof(RunDuration));
            };
            HeatingWatch.StopAfter(SetDuration * 1000);
            HeatingWatch.StartAsync().ConfigureAwait(false);
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
