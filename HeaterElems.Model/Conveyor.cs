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


        public event EventHandler<BoardArgs> BoardDispensed;

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
        private ObservableCollection<Station> _stationOrderedList;
        public ObservableCollection<Station> StationOrderedList
        {
            get { return _stationOrderedList ?? (_stationOrderedList = new ObservableCollection<Station>()); }
            set { SetProperty(ref _stationOrderedList, value); }
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
