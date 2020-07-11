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
    public class DispensingWorkStationViewModel : ViewModelBase<DispensingWorkStation>
    {
        #region properties

        #region ModelContext
        public new DispensingWorkStation ModelContext => DispensingWorkStation.Instance;
        #endregion ModelContext

        #region RunCommand
        public RelayCommand RunCommand => new RelayCommand(async (o) => { HasStopped = false; await RunAsync(); });
        #endregion RunCommand

        #region StopCommand
        public RelayCommand StopCommand => new RelayCommand((o) => HasStopped = true);
        #endregion StopCommand

        #region StepCommand
        public RelayCommand StepCommand => new RelayCommand((o) => { Step(); });
        #endregion StepCommand

        #region NumberOfStations
        private int _numberOfStations;

        public int NumberOfStations {
            get { return _numberOfStations; }
            set
            {
                SetProperty(ref _numberOfStations, value);
                ConveyorViewModelsList.ForEach(c => c.NumberOfStations = NumberOfStations);
            }
        }
        #endregion NumberOfStations

        #region CurrentBoardId
        public int CurrentBoardId = 0;
        #endregion CurrentBoardId

        #region HasStopped
        private bool _hasStopped;
        public bool HasStopped
        {
            get { return _hasStopped; }
            set { SetProperty(ref _hasStopped, value); }
        }
        #endregion HasStopped

        #region ConveyorViewModelsList
        private List<ConveyorViewModel> _conveyorViewModelsList;
        public List<ConveyorViewModel> ConveyorViewModelsList
        {
            get {
                if (_conveyorViewModelsList != null) return _conveyorViewModelsList;

                var conveyorViewModelsList = new List<ConveyorViewModel>();
                var conv1 = new ConveyorViewModel() { ModelContext = new Conveyor("Back Lane", 2) };
                conveyorViewModelsList.Add(conv1);
                var conv2 = new ConveyorViewModel() {ModelContext = new Conveyor("Front Lane", 1) };
                conveyorViewModelsList.Add(conv2);
                ConveyorViewModelsList = conveyorViewModelsList;

                return _conveyorViewModelsList;
            }
            set { SetProperty(ref _conveyorViewModelsList, value); }
        }
        #endregion ConveyorViewModelsList

        #region DispensedWorkPiecesViewModel
        private DispensedWorkPiecesContainerViewModel _dispensedWorkPiecesViewModel;
        public DispensedWorkPiecesContainerViewModel DispensedWorkPiecesViewModel
        {
            get { return _dispensedWorkPiecesViewModel ?? (_dispensedWorkPiecesViewModel = new DispensedWorkPiecesContainerViewModel() {ModelContext = new DispensedWorkPiecesContainer()}); }
            set { SetProperty(ref _dispensedWorkPiecesViewModel, value); }
        }
        #endregion DispensedWorkPiecesViewModel

        #endregion properties

        #region constructors
        public DispensingWorkStationViewModel() {
            base.ModelContext = DispensingWorkStation.Instance;
            this.PropertyChanged += DispensingWorkStationViewModel_PropertyChanged;
            NumberOfStations = 3;
        }
        private void DispensingWorkStationViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ConveyorViewModelsList)) return;

            ConveyorViewModelsList?.ForEach(c => c.ModelContext.BoardDispensed += Conveyor_BoardDispensed); //this creates a memory leak
        }

        public void Conveyor_BoardDispensed(object sender, WorkPiece e)
        {
            var dispensedWorkPieceVm = new WorkPieceViewModel() {WorkPiece = e};
            DispensedWorkPiecesViewModel.DispensedBoardsVmList.Insert(0, dispensedWorkPieceVm);
            
        }

        #endregion constructors

        public async Task RunAsync()
        {
            while (HasStopped == false)
            {
                Step();
                await Task.Delay(70);
            }
        }

        public void Step()
        {
            // Load a board on the first PreDispensing Station that is empty
            var firstAvailableLane = ConveyorViewModelsList.FirstOrDefault(c => c.CanLoadBoard);
            if (firstAvailableLane == null) return;
            CurrentBoardId++;
            var board = new WorkPiece(CurrentBoardId.ToString());
            firstAvailableLane?.LoadBoard(board);
        }
    }
}
    
