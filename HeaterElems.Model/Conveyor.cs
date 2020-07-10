using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;

namespace HeaterElems.Model
{
    public class Conveyor : SetPropertyBase
    {


        public event EventHandler<WorkPiece> BoardDispensed;

        #region Name
        private string _name;
        public string Name {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        #endregion Name

        #region LaneNumber
        private int _laneNumber;
        public int LaneNumber
        {
            get { return _laneNumber; }
            set { SetProperty(ref _laneNumber, value); }
        }
        #endregion LaneNumber

        #region StationOrderedList
        private IReadOnlyList<Station> _stationOrderedList;
        public IReadOnlyList<Station> StationOrderedList
        {
            get { return _stationOrderedList ?? (_stationOrderedList = new List<Station>()); }
            set
            {

                SetProperty(ref _stationOrderedList, value);
                var lastStation = _stationOrderedList?.LastOrDefault();
                if (lastStation != null) lastStation.WorkPieceUnloaded += (s, wp) => BoardDispensed?.Invoke(this, wp);
            }
        }

        #endregion StationViewModelsOrderedLis

        #region constructor
        public Conveyor(string name, int laneNumber)
        {
            Name = name;
            LaneNumber = laneNumber;
        }
        #endregion constructor

    }
}
