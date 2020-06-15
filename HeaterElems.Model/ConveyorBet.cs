using System;
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

        #region MainStations
        private ObservableCollection<Station> _mainStations;
        public ObservableCollection<Station> MainStations {
            get => _mainStations ?? (_mainStations = new ObservableCollection<Station>() {new Station() {Name = "MainStation"}});
            set => SetProperty(ref _mainStations, value);
        }
        #endregion MainStations

        #region PostStation
        private Station _postStation;
        public Station PostStation {
            get { return _postStation ?? (_postStation = new Station() { Name = nameof(PostStation) }); }
            set { SetProperty(ref _postStation, value); }
        }
        #endregion PostStation
 
    }
}
