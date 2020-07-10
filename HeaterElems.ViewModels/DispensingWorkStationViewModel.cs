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
        public RelayCommand RunCommand => new RelayCommand((o) => { /*Run()*/ });
        #endregion RunCommand

        #region StopCommand
        public RelayCommand StopCommand => new RelayCommand((o) => HasStopped = true);
        #endregion StopCommand

        #region StepCommand
        public RelayCommand StepCommand => new RelayCommand((o) => { Step(); });
        #endregion StepCommand

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
        private List<ConveyorBeltViewModel> _conveyorViewModelsList;
        public List<ConveyorBeltViewModel> ConveyorViewModelsList
        {
            get {
                if (_conveyorViewModelsList != null) return _conveyorViewModelsList;

                var conveyorViewModelsList = new List<ConveyorBeltViewModel>();
                var conv1 = new ConveyorBeltViewModel() { ModelContext = new Conveyor("Back Lane", 2) };
                conveyorViewModelsList.Add(conv1);
                var conv2 = new ConveyorBeltViewModel() {ModelContext = new Conveyor("Front Lane", 1) };
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
        }
        private void DispensingWorkStationViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ConveyorViewModelsList)) return;

            ConveyorViewModelsList?.ForEach(c => c.ModelContext.BoardDispensed += Conveyor_BoardDispensed); //this creates a memory leak
        }

        private void Conveyor_BoardDispensed(object sender, WorkPiece e)
        {
            var dispensedWorkPieceVm = new WorkPieceViewModel() {WorkPiece = e};
            DispensedWorkPiecesViewModel.DispensedBoardsVmList.Add(dispensedWorkPieceVm);
            
        }
        #endregion constructors



        private void Step()
        {
            CurrentBoardId++;
            // Load a board on the first PreDispensing Station that is empty
            var firstAvailableLane = ConveyorViewModelsList.FirstOrDefault(c => c.CanLoadBoard);
            var board = new WorkPiece(CurrentBoardId.ToString());
            firstAvailableLane?.LoadBoard(board);
        }
    }
}
    
