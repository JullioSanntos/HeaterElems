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

        //#region PreStation
        //public const string PreStationName = nameof(PreStation);
        //private Station _preStation;
        //public Station PreStation {
        //    get { return _preStation ?? (_preStation = new Station() {Name = PreStationName}); }
        //    set { SetProperty(ref _preStation, value); }
        //}
        //#endregion PreStation

        //#region MainStation
        //public const string MainStationName = nameof(MainStation);
        //private Station _mainStation;
        //public Station MainStation {
        //    get { return _mainStation ?? (_mainStation = new Station() {Name = MainStationName}); }
        //    set { SetProperty(ref _mainStation, value); }
        //}
        //#endregion MainStation

        //#region PostStation

        //public static string PostStationName = nameof(PostStation);
        //private Station _postStation;
        //public Station PostStation {
        //    get { return _postStation ?? (_postStation = new Station() {Name = PostStationName}); }
        //    set { SetProperty(ref _postStation, value); }
        //}
        //#endregion PostStation

        public event EventHandler<BoardArgs> BoardDispensed;



        #region constructor
        public Conveyor(string name, int laneNumber)
        {
            Name = name;
            LaneNumber = laneNumber;
            //this.PreStation.WorkPieceLoaded += async (s, e) => await PreStation_BoardLoaded(s, e);
            //this.MainStation.WorkPieceLoaded += async (s, e) => await MainStation_BoardLoaded(s, e);
            //this.PostStation.WorkPieceLoaded += async (s, e) => await PostStation_BoardLoaded(s, e);
        }

        //public Conveyor()
        //{
        //    this.PreStation.WorkPieceLoaded += async (s,e) => await PreStation_BoardLoaded(s,e);
        //    this.MainStation.WorkPieceLoaded += async (s,e) => await MainStation_BoardLoaded(s,e);
        //    this.PostStation.WorkPieceLoaded += async (s,e) => await PostStation_BoardLoaded(s,e);
        //}

        //private async Task PreStation_BoardLoaded(object sender, BoardArgs e)
        //{

        //    await Task.Delay(1500);
        //    MainStation.WorkPiece = e.WorkPiece;
        //    PreStation.WorkPiece = null;

        //}

        //private async Task MainStation_BoardLoaded(object sender, BoardArgs e)
        //{

        //    await Task.Delay(1500);
        //    PostStation.WorkPiece = e.WorkPiece;
        //    MainStation.WorkPiece = null;
        //}

        //private async Task PostStation_BoardLoaded(object sender, BoardArgs e)
        //{
        //    await Task.Delay(1500);
        //    PostStation.WorkPiece = null;
        //    BoardDispensed?.Invoke(this, new BoardArgs(PostStation, e.WorkPiece));

        //}

        #endregion constructor

    }
}
