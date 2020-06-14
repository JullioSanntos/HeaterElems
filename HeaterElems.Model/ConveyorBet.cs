using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;

namespace HeaterElems.Model
{
    public class ConveyorBet : SetPropertyBase
    {

        #region PreStation
        private Station _preStation;
        public Station PreStation {
            get { return _preStation ?? (_preStation = new Station()); }
            set { SetProperty(ref _preStation, value); }
        }
        #endregion PreStation

        #region Stations
        private ObservableCollection<Station> _stations;
        public ObservableCollection<Station> Stations {
            get => _stations ?? (_stations = new ObservableCollection<Station>());
            set => SetProperty(ref _stations, value);
        }
        #endregion Stations

        #region PostStation
        private Station _postStation;
        public Station PostStation {
            get { return _postStation ?? (_postStation = new Station()); }
            set { SetProperty(ref _postStation, value); }
        }
        #endregion PostStation
 
    }
}
