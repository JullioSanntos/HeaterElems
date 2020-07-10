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
                        var station = new Station(stationIx, GetStationType(NumberOfStations, stationIx));

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

        #region PreStationVm
        public StationViewModel PreStationVm {
            get { return StationViewModelsOrderedList?.OrderBy(s => s.ModelContext.DownstreamOrder).First(); }
        }
        #endregion PreStationVm

        #region CanLoadBoard
        public bool CanLoadBoard { get { return PreStationVm?.ModelContext?.HasBoard == false; } }
        #endregion CanLoadBoard

        #region constructor
        public ConveyorViewModel() {
            this.PropertyChanged += ConveyorBeltViewModel_PropertyChanged;
        }
        #endregion constructor

        private void ConveyorBeltViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(NumberOfStations):
                    // if number of Stations is changed reset the OrderedList so that it can be lazy instantiated
                    StationViewModelsOrderedList = null;
                    break;
                case nameof(StationViewModelsOrderedList):
                    RaisePropertyChanged(nameof(PreStationVm));
                    // this will create a memory leak but this is just demo code. This code move unloading boards to the next station
                    for (var i = 1; i < NumberOfStations; i++) {
                        var ix = i;
                        StationViewModelsOrderedList[ix - 1].ModelContext.WorkPieceUnloaded += async (s, wp) => await StationViewModelsOrderedList[ix].LoadBoardAsync(wp);
                    }
                    break;
                case nameof(PreStationVm):
                    RaisePropertyChanged(nameof(CanLoadBoard));
                    break;
            }
        }

        public void LoadBoard(WorkPiece board)
        {
            PreStationVm?.LoadBoardAsync(board);
        }

        protected StationTypeEnum GetStationType(int nbrOfStations, int stationIx)
        {
            var stationType = StationTypeEnum.DispensingStation;
            switch (nbrOfStations)
            {
                case 1:
                    stationType = StationTypeEnum.DispensingStation;
                    break;
                case 2:
                    if (stationIx == 0) stationType = StationTypeEnum.PreStation;
                    else stationType = StationTypeEnum.DispensingStation;
                    break;
                default:
                    if (stationIx == 0) stationType = StationTypeEnum.PreStation;
                    else if (stationIx == nbrOfStations - 1) stationType = StationTypeEnum.PostStation;
                    else stationType = StationTypeEnum.DispensingStation;
                    break;
                
            }

            return stationType;
        }

    }
}
