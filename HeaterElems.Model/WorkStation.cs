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
        private ObservableCollection<Transport> _conveyorBets;
        public ObservableCollection<Transport> ConveyorBets {
            get => _conveyorBets;
            set => SetProperty(ref _conveyorBets, value);
        }
        #endregion ConveyorBets
    }
}
