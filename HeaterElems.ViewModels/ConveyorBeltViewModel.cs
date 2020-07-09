using HeaterElems.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;

namespace HeaterElems.ViewModels
{
    public class ConveyorBeltViewModel : ViewModelBase<Conveyor>
    {


        #region NumberOfStations
        private int _numberOfStations = 3;

        public int NumberOfStations
        {
            get { return _numberOfStations; }
            set { SetProperty(ref _numberOfStations, value); }
        }
        #endregion NumberOfStations

        #region StationViewModelsOrderedList
        private ObservableCollection<StationViewModel> _stationViewModelsOrderedList;
        public ObservableCollection<StationViewModel> StationViewModelsOrderedList {
            get
            {
                if (_stationViewModelsOrderedList == null)
                {
                    StationViewModelsOrderedList = new ObservableCollection<StationViewModel>();

                    for (int stationIx = 0; stationIx < NumberOfStations; stationIx++) {
                        Station station;
                        if (stationIx == 0) station = new Station(stationIx, StationTypeEnum.PreStation);
                        else if (stationIx == NumberOfStations - 1) station = new Station(stationIx, StationTypeEnum.PostStation);
                        else station = new Station(stationIx, StationTypeEnum.DispensingStation);

                        StationViewModelsOrderedList.Add(new StationViewModel() {ModelContext = station});
                    }
                }

                return _stationViewModelsOrderedList; 
            }
            private set { SetProperty(ref _stationViewModelsOrderedList, value); }
        }
        #endregion StationViewModelsOrderedList


        public ConveyorBeltViewModel()
        {
            this.PropertyChanged += (s, e) =>
            {
                // if number of Stations is changed reset the OrderedList so that it can be lazy instantiated
                if (e.PropertyName == nameof(NumberOfStations)) StationViewModelsOrderedList = null;
            };
        }

        public void LoadBoard(int boardId)
        {
            StationViewModelsOrderedList.First()?.LoadBoard(boardId);
        }
    }
}
