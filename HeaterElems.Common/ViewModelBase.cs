using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeaterElems.Common
{
    public class ViewModelBase<T>: SetPropertyBase
    {
        #region ModelContext
        private T _modelContext;

        public T ModelContext
        {
            get { return _modelContext; }
            set { SetProperty(ref _modelContext, value); }
        }
        #endregion ModelContext

        #region ModelContextObject
        public object ModelContextObject
        {
            get { return ModelContext as object; }
            set
            {
                ModelContext = (T)value;
                RaisePropertyChanged(nameof(ModelContextObject));
            }
        }
        #endregion ModelContextObject
    }
}
