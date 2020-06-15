﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;

namespace HeaterElems.Model
{
    public class ConveyorBelt : SetPropertyBase
    {

        #region Name
        private string _name;
        public string Name {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        #endregion Name

        #region PreStation
        private Station _preStation;
        public Station PreStation {
            get { return _preStation ?? (_preStation = new Station() {Name = nameof(PreStation)}); }
            set { SetProperty(ref _preStation, value); }
        }
        #endregion PreStation

        #region MainStation
        private Station _mainStation;
        public Station MainStation {
            get { return _mainStation ?? (_mainStation = new Station() { Name = nameof(MainStation) }); }
            set => SetProperty(ref _mainStation, value);
        }
        #endregion MainStation

        #region PostStation
        private Station _postStation;
        public Station PostStation {
            get { return _postStation ?? (_postStation = new Station() { Name = nameof(PostStation) }); }
            set { SetProperty(ref _postStation, value); }
        }
        #endregion PostStation

        public ConveyorBelt() {
            this.PreStation.PropertyChanged += PreStation_PropertyChanged;
            this.PostStation.PropertyChanged += PostStation_PropertyChanged;
        }


        private void PreStation_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(PreStation.Board):
                    if (PreStation.Board != null) {
                        MainStation.Board = PreStation.Board;
                        PreStation.Board = null;
                    }
                    break;
            }
        }

        private void PostStation_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }
    }
}
