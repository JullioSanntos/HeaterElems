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
    public class ConveyorBeltViewModel : SetPropertyBase
    {
        #region ModelContext
        private ConveyorBelt _modelContext;
        public ConveyorBelt ModelContext {
            get => _modelContext;
            set => SetProperty(ref _modelContext, value);
        }
        #endregion ModelContext

        #region StationViewModelsList
        private ObservableCollection<StationViewModel> _stationViewModelsList;
        public ObservableCollection<StationViewModel> StationViewModelsList {
            get => _stationViewModelsList;
            set => SetProperty(ref _stationViewModelsList, value);
        }
        #endregion StationViewModelsList

        //public ConveyorBeltViewModel() {
        //    this.PropertyChanged += ConveyorBeltViewModel_PropertyChanged;
        //}

        //private void ConveyorBeltViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == nameof(ModelContext))
        //    {
        //        ModelContext.PreStation = new Station();
        //        ModelContext.PostStation = new Station();
        //        ModelContext.MainStation = new Station();
        //    }
        //}
    }
}
