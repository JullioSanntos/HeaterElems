using HeaterElems.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;

namespace HeaterElems.ViewModels
{
    public class ConveyorBeltViewModel : SetPropertyBase
    {
        #region ModelContext
        private ConveyorBelt _modelContext;
        public ConveyorBelt ModelContext {
            get => _modelContext;
            set => SetProperty(ref _modelContext, value);
        }
        #endregion ModelContext
    }
}
