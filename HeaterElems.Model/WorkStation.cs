using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;

namespace HeaterElems.Model
{
    public class WorkStation : SetPropertyBase
    {
        #region ConveyorBets
        private ObservableCollection<ConveyorBet> _conveyorBets;
        public ObservableCollection<ConveyorBet> ConveyorBets {
            get => _conveyorBets;
            set => SetProperty(ref _conveyorBets, value);
        }
        #endregion ConveyorBets
    }
}
