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

        //#region ConveyorBeltViewModels
        //private ObservableCollection<ConveyorBeltViewModel> _conveyorBeltViewModels;
        //public ObservableCollection<ConveyorBeltViewModel> ConveyorBeltViewModels {
        //    get {
        //        if (_conveyorBeltViewModels == null)
        //        {
        //            ModelContext.ConveyorBets = new ObservableCollection<ConveyorBelt>(){
        //                new ConveyorBelt() {Name = "Front Lane"}, new ConveyorBelt() {Name = "Back Lane"}
        //            };
        //            ConveyorBeltViewModels = new ObservableCollection<ConveyorBeltViewModel>(){
        //                new ConveyorBeltViewModel(ModelContext.ConveyorBets[0])
        //                , new ConveyorBeltViewModel(ModelContext.ConveyorBets[1])
        //            };
        //        }
        //        return _conveyorBeltViewModels; 
        //    }
        //    set => SetProperty(ref _conveyorBeltViewModels, value);
        //}
        //#endregion ConveyorBeltViewModels


        //#region Singleton        
        //private static readonly object SingletonLock = new object();
        //private static DispensingWorkStationViewModel _instance;

        //public static DispensingWorkStationViewModel Instance {
        //    get { return _instance ?? (_instance = GetSingleton()); }
        //}

        //public static DispensingWorkStationViewModel GetSingleton() {
        //    if (_instance != null) return _instance;
        //    lock (SingletonLock)
        //    {
        //        return _instance ?? (_instance = new DispensingWorkStationViewModel());
        //    }
        //}

        //private DispensingWorkStationViewModel() {
        //}
        //#endregion Singleton
    }
}
