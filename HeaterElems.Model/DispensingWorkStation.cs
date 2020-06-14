using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;

namespace HeaterElems.Model
{
    public class DispensingWorkStation : SetPropertyBase
    {
        #region ConveyorBets
        private ObservableCollection<ConveyorBelt> _conveyorBets;
        public ObservableCollection<ConveyorBelt> ConveyorBets {
            get => _conveyorBets;
            set => SetProperty(ref _conveyorBets, value);
        }
        #endregion ConveyorBets

        #region DispensedBoardsContainer
        private ObservableCollection<DispensingBoardsContainer> _dispensedBoardsContainer;
        public ObservableCollection<DispensingBoardsContainer> DispensedBoardsContainer {
            get => _dispensedBoardsContainer ?? (_dispensedBoardsContainer = new ObservableCollection<DispensingBoardsContainer>());
            set => SetProperty(ref _dispensedBoardsContainer, value);
        }
        #endregion DispensedBoardsContainer



        //#region Singleton        
        //private static readonly object SingletonLock = new object();
        //private static DispensingWorkStation _dispensingWorkStation;

        //public static DispensingWorkStation Instance {
        //    get { return _dispensingWorkStation ?? (_dispensingWorkStation = GetSingleton()); }
        //}

        //public static DispensingWorkStation GetSingleton()
        //{
        //    if (_dispensingWorkStation != null) return _dispensingWorkStation;
        //    lock (SingletonLock)
        //    {
        //        return _dispensingWorkStation ?? (_dispensingWorkStation = new DispensingWorkStation());
        //    }
        //}

        //private DispensingWorkStation() {  }
        //#endregion Singleton
    }
}
