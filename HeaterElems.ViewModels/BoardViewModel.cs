using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;
using HeaterElems.Model;

namespace HeaterElems.ViewModels
{
    public class BoardViewModel : SetPropertyBase
    {
        #region ModelContext
        private Board _modelContext;
        public Board ModelContext {
            get => _modelContext;
            set => SetProperty(ref _modelContext, value);
        }
        #endregion ModelContext
    }
}
