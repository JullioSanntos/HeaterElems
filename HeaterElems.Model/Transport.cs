using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;

namespace HeaterElems.Model
{
    public class Transport : SetPropertyBase
    {
        #region ConveyorBets
        private ObservableCollection<Heater> _heatingParts;
        public ObservableCollection<Heater> HeatingParts {
            get => _heatingParts;
            set => SetProperty(ref _heatingParts, value);
        }
        #endregion ConveyorBets
    }
}
