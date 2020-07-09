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

                _conveyorViewModelsList = new List<ConveyorBeltViewModel>();
                var conv1 = new ConveyorBeltViewModel() { ModelContext = new Conveyor("Back Lane", 2) };
                _conveyorViewModelsList.Add(conv1);
                var conv2 = new ConveyorBeltViewModel() {ModelContext = new Conveyor("Front Lane", 1) };
                _conveyorViewModelsList.Add(conv2);
                return _conveyorViewModelsList;
            }
            set { _conveyorViewModelsList = value; }
        }
        #endregion ConveyorViewModelsList


        #endregion properties

        #region constructors
        public DispensingWorkStationViewModel() {
            base.ModelContext = DispensingWorkStation.Instance;
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
    
