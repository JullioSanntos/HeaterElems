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
        public DispensingWorkStation ModelContext => DispensingWorkStation.Instance;

        #region ConveyorBeltViewModels
        private ObservableCollection<ConveyorBeltViewModel> _conveyorBeltViewModels;
        public ObservableCollection<ConveyorBeltViewModel> ConveyorBeltViewModels {
            get {
                if (_conveyorBeltViewModels == null)
                {
                    ModelContext.ConveyorBets = new ObservableCollection<ConveyorBelt>(){
                        new ConveyorBelt() {Name = "Front Lane"}, new ConveyorBelt() {Name = "Back Lane"}
                    };
                    ConveyorBeltViewModels = new ObservableCollection<ConveyorBeltViewModel>(){
                        new ConveyorBeltViewModel() {ModelContext = ModelContext.ConveyorBets[0]}
                        , new ConveyorBeltViewModel() {ModelContext = ModelContext.ConveyorBets[1]}
                    };
                }
                return _conveyorBeltViewModels; 
            }
            set => SetProperty(ref _conveyorBeltViewModels, value);
        }
        #endregion ConveyorBeltViewModels

    }
}
