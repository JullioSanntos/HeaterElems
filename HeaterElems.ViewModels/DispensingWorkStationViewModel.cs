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
    public class DispensingWorkStationViewModel : SetPropertyBase
    {
        #region properties

        public DispensingWorkStation ModelContext => DispensingWorkStation.Instance;


        public RelayCommand RunCommand => new RelayCommand((o) =>
        {
            /*Run()*/
        });
        public RelayCommand StopCommand => new RelayCommand((o) => HasStopped = true);
        public RelayCommand StepCommand => new RelayCommand((o) =>
        {
            Step();
        });

        public int CurrentBoardId = 0;

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



        private void Step()
        {
            CurrentBoardId++;
            // Load a board on the first PreDispensing Station that is empty
            ConveyorViewModelsList.FirstOrDefault(c => c.StationViewModelsOrderedList?.First()?.HasBoard == false)?.LoadBoard(CurrentBoardId);
        }
    }
}
    
