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
    public class ConveyorViewModel : ViewModelBase<Conveyor>
    {

        #region NumberOfStations
        private int _numberOfStations;

        public int NumberOfStations
        {
            get { return _numberOfStations; }
            set { SetProperty(ref _numberOfStations, value); }
        }
        #endregion NumberOfStations

        #region StationViewModelsOrderedList
        private IReadOnlyList<StationViewModel> _stationViewModelsOrderedList;
        public IReadOnlyList<StationViewModel> StationViewModelsOrderedList {
            get
            {
                if (_stationViewModelsOrderedList == null)
                {
                    var stationViewModelsOrderedList = new List<StationViewModel>();
                    ModelContext.StationOrderedList = null;
                    var stationOrderedList = new List<Station>();

                    for (int stationIx = 0; stationIx < NumberOfStations; stationIx++) {
                        Station station;
                        if (stationIx == 0) station = new Station(stationIx, StationTypeEnum.PreStation);
                        else if (stationIx == NumberOfStations - 1) station = new Station(stationIx, StationTypeEnum.PostStation);
                        else station = new Station(stationIx, StationTypeEnum.DispensingStation);

                        stationOrderedList.Add(station);
                        stationViewModelsOrderedList.Add(new StationViewModel() {ModelContext = station});
                    }

                    ModelContext.StationOrderedList = stationOrderedList;
                    StationViewModelsOrderedList = new List<StationViewModel>(stationViewModelsOrderedList);
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

        public ConveyorViewModel()
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
                    // this will create a memory leak but this is just demo code. This code move unloading boards to the next station
                    for (var i = 1; i < NumberOfStations; i++) {
                        var ix = i;
                        StationViewModelsOrderedList[ix - 1].ModelContext.WorkPieceUnloaded += async (s, wp) => await StationViewModelsOrderedList[ix].LoadBoardAsync(wp);
                    }
                    break;
                case nameof(PreStationVM):
                    RaisePropertyChanged(nameof(CanLoadBoard));
                    break;
            }
        }


        public void LoadBoard(WorkPiece board)
        {
            PreStationVM?.LoadBoardAsync(board);
        }

    }
}
