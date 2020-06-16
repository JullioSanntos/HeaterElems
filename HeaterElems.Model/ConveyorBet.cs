using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;

namespace HeaterElems.Model
{
    public class ConveyorBelt : SetPropertyBase
    {

        #region Name
        private string _name;
        public string Name {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        #endregion Name

        #region PreStation
        private Station _preStation;
        public Station PreStation {
            get { return _preStation ?? (_preStation = new Station() {Name = nameof(PreStation)}); }
            set { SetProperty(ref _preStation, value); }
        }
        #endregion PreStation

        #region MainStation
        private Station _mainStation;
        public Station MainStation {
            get { return _mainStation ?? (_mainStation = new Station() { Name = nameof(MainStation) }); }
            set => SetProperty(ref _mainStation, value);
        }
        #endregion MainStation

        #region PostStation
        private Station _postStation;
        public Station PostStation {
            get { return _postStation ?? (_postStation = new Station() { Name = nameof(PostStation) }); }
            set { SetProperty(ref _postStation, value); }
        }
        #endregion PostStation


        public event EventHandler<BoardArgs> BoardUnloaded;
        public event EventHandler<ConveyorArgs> BoardsMoved;

        #region constructor
        public ConveyorBelt() {
            this.PreStation.PropertyChanged += async (s, e) => {
                if (e.PropertyName == nameof(PreStation.Board))
                    await MarchBoardsForward(this);
            };

            this.PostStation.PropertyChanged += async (s, e) => {
                if (e.PropertyName == nameof(PostStation.Board))
                    await MarchBoardsForward(this);
            };
        }
        #endregion constructor


        //private async Task StationChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
        //    if (e.PropertyName != nameof(PreStation.Board)) return;
        //    await MarchBoardsForward();
        //}


        private async Task MarchBoardsForward(ConveyorBelt lane) {
            await Task.Delay(1500);
            var dispensedBoard = PostStation.Board;
            if (PostStation.Board != null) {
                PostStation.Board.ProgressiveTimer.Stop();
                BoardUnloaded?.Invoke(this, new BoardArgs(dispensedBoard)); 
                PostStation.Board = null;
            }

            PostStation.Board = MainStation.Board;
            MainStation.Board = PreStation.Board;
            PreStation.Board = null;
            BoardsMoved?.Invoke(this,new ConveyorArgs(lane));
        }


    }
}
