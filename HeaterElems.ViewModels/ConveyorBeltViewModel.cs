using HeaterElems.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
                    ModelContext.StationOrderedList.Clear();

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

        #region PreStationVM
        public StationViewModel PreStationVM { get { return StationViewModelsOrderedList?.FirstOrDefault(c => c.ModelContext.StationType == StationTypeEnum.PreStation); } }
        #endregion PreStationVM

        #region CanLoadBoard
        public bool CanLoadBoard { get { return PreStationVM?.ModelContext?.HasBoard == false; } }
        #endregion CanLoadBoard

        public ConveyorBeltViewModel()
        {
            this.PropertyChanged += ConveyorBeltViewModel_PropertyChanged;
        }

        private void ConveyorBeltViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(NumberOfStations):
                    // if number of Stations is changed reset the OrderedList so that it can be lazy instantiated
                    StationViewModelsOrderedList = null;
                    break;
                case nameof(StationViewModelsOrderedList):
                    RaisePropertyChanged(nameof(PreStationVM));
                    break;
                case nameof(PreStationVM):
                    RaisePropertyChanged(nameof(CanLoadBoard));
                    break;
            }
        }


        public void LoadBoard(int boardId)
        {
            PreStationVM?.LoadBoardAsync(boardId);
        }
    }
}
