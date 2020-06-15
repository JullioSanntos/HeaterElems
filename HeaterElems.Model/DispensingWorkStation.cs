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
            get {
                if (_conveyorBets == null)
                {
                    ConveyorBets = new ObservableCollection<ConveyorBelt>(){
                            new ConveyorBelt() {Name = "Front Lane"}, new ConveyorBelt() {Name = "Back Lane"}
                        };
                }
                return _conveyorBets;
            }
            set => SetProperty(ref _conveyorBets, value);
        }
        #endregion ConveyorBets

        #region BoardsContainer
        private BoardsContainer _boardsContainer;
        public BoardsContainer BoardsContainer {
            get => _boardsContainer ?? (_boardsContainer = new BoardsContainer());
            set => SetProperty(ref _boardsContainer, value);
        }
        #endregion BoardsContainer

        #region Singleton        
        private static readonly object SingletonLock = new object();
        private static DispensingWorkStation _dispensingWorkStation;

        public static DispensingWorkStation Instance {
            get { return _dispensingWorkStation ?? (_dispensingWorkStation = GetSingleton()); }
        }

        public static DispensingWorkStation GetSingleton()
        {
            if (_dispensingWorkStation != null) return _dispensingWorkStation;
            lock (SingletonLock)
            {
                return _dispensingWorkStation ?? (_dispensingWorkStation = new DispensingWorkStation());
            }
        }

        private DispensingWorkStation() { ConveyorBets.ToList().ForEach(cb => cb.BoardUnloaded += BoardDispensed);}
        #endregion Singleton


        private void BoardDispensed(object sender, BoardArgs e)
        {
            this.BoardsContainer.DispensedBoards.Insert(0, e.Board);
        }
    }
}
