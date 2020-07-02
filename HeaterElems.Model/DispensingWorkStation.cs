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
        #region Conveyors
        private ObservableCollection<Conveyor> _conveyors;
        public ObservableCollection<Conveyor> Conveyors {
            get {
                if (_conveyors == null)
                {
                    Conveyors = new ObservableCollection<Conveyor>(){
                            new Conveyor() {Name = "Front Lane"}, new Conveyor() {Name = "Back Lane"}
                        };
                }
                return _conveyors;
            }
            set => SetProperty(ref _conveyors, value);
        }
        #endregion Conveyors

        #region DispensedWorkPiecesContainer
        private DispensedWorkPiecesContainer _dispensedWorkPiecesContainer;
        public DispensedWorkPiecesContainer DispensedWorkPiecesContainer {
            get => _dispensedWorkPiecesContainer ?? (_dispensedWorkPiecesContainer = new DispensedWorkPiecesContainer());
            set => SetProperty(ref _dispensedWorkPiecesContainer, value);
        }
        #endregion DispensedWorkPiecesContainer

        #region Singleton        
        private static readonly object SingletonLock = new object();
        private static DispensingWorkStation _dispensingWorkStation;

        public static DispensingWorkStation Instance => _dispensingWorkStation ?? (_dispensingWorkStation = GetSingleton());

        public static DispensingWorkStation GetSingleton()
        {
            if (_dispensingWorkStation != null) return _dispensingWorkStation;
            lock (SingletonLock)
            {
                return _dispensingWorkStation ?? (_dispensingWorkStation = new DispensingWorkStation());
            }
        }

        private DispensingWorkStation()
        {
            Conveyors.ToList().ForEach(cb => cb.BoardDispensed += BoardDispensed);
        }
        #endregion Singleton

        private void BoardDispensed(object sender, BoardArgs e)
        {
            if (e.Station.Name == Conveyor.PostStationName)
                this.DispensedWorkPiecesContainer.DispensedBoards.Insert(0, e.WorkPiece);
        }
    }
}
