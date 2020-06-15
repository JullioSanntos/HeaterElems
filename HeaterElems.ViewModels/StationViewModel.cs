using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;
using HeaterElems.Model;

namespace HeaterElems.ViewModels
{
    public class StationViewModel : SetPropertyBase
    {
        
        #region ModelContext
        private Station _modelContext;
        public Station ModelContext {
            get => _modelContext;
            set => SetProperty(ref _modelContext, value);
        }
        #endregion ModelContext

        #region ModelContext
        public bool HasBoard => ModelContext.Board != null;
        #endregion ModelContext

        public StationViewModel() {
            this.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(ModelContext) && ModelContext != null)
                {
                    ModelContext.PropertyChanged += ModelContext_PropertyChanged;
                    RaisePropertyChanged(nameof(HasBoard));
                }
            };

        }

        private void ModelContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ModelContext.Board):
                    RaisePropertyChanged(nameof(HasBoard));
                    break;
            }
        }
    }
}
