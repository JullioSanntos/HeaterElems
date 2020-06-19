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
            get => _name;
            set => SetProperty(ref _name, value);
        }
        #endregion Name

        #region PreStation
        public const string PreStationName = nameof(PreStation);
        private Station _preStation;
        public Station PreStation {
            get => _preStation ?? (_preStation = new Station() {Name = PreStationName });
            set => SetProperty(ref _preStation, value);
        }
        #endregion PreStation

        #region MainStation
        public const string MainStationName = nameof(MainStation);
        private Station _mainStation;
        public Station MainStation {
            get => _mainStation ?? (_mainStation = new Station() { Name = MainStationName });
            set => SetProperty(ref _mainStation, value);
        }
        #endregion MainStation

        #region PostStation

        public static string PostStationName = nameof(PostStation);
        private Station _postStation;
        public Station PostStation {
            get => _postStation ?? (_postStation = new Station() { Name = PostStationName });
            set => SetProperty(ref _postStation, value);
        }
        #endregion PostStation

        public event EventHandler<BoardArgs> BoardDispensed;

 

        #region constructor
        //public Conveyor() {
        //    this.PreStation.PropertyChanged += async (s, e) => {
        //        if (e.PropertyName == nameof(PreStation.Board))
        //            await BoardLoaded(this, this.PreStation);
        //    };

        //    this.MainStation.PropertyChanged += async (s, e) => {
        //        if (e.PropertyName == nameof(PreStation.Board))
        //            await BoardLoaded(this, this.MainStation);
        //    };

        //    this.PostStation.PropertyChanged += async (s, e) => {
        //        if (e.PropertyName == nameof(PostStation.Board))
        //            await BoardLoaded(this, this.PostStation);
        //    };
        //}
        public Conveyor()
        {
            this.PreStation.BoardLoaded += async (s,e) => await PreStation_BoardLoaded(s,e);
            this.MainStation.BoardLoaded += async (s,e) => await MainStation_BoardLoaded(s,e);
            this.PostStation.BoardLoaded += async (s,e) => await PostStation_BoardLoaded(s,e);
        }

        private async Task PreStation_BoardLoaded(object sender, BoardArgs e)
        {

            await Task.Delay(1500);
            MainStation.Board = e.Board;
            PreStation.Board = null;

        }

        private async Task MainStation_BoardLoaded(object sender, BoardArgs e)
        {
            
            await Task.Delay(1500);
            PostStation.Board = e.Board;
            MainStation.Board = null;
        }

        private async Task PostStation_BoardLoaded(object sender, BoardArgs e)
        {
            await Task.Delay(1500);
            PostStation.Board = null;
            BoardDispensed?.Invoke(this, new BoardArgs(PostStation, e.Board));

        }

        #endregion constructor
        }
}
