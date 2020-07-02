using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;

namespace HeaterElems.Model
{
    public class Station : SetPropertyBase
    {

        #region Name
        private string _name;
        public string Name {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        #endregion Name

        #region Heater
        private Heater _heater;
        public Heater Heater {
            get { return _heater ?? (_heater = new Heater()); }
            set { SetProperty(ref _heater, value); }
        }
        #endregion Heater

        #region WorkPiece
        private WorkPiece _workPiece;
        public WorkPiece WorkPiece {
            get { return _workPiece; }
            set
            {
                var previousBoard = _workPiece;
                SetProperty(ref _workPiece, value);
                if (_workPiece == null) WorkPieceUnloaded?.Invoke(this, new BoardArgs(null, previousBoard));
                else WorkPieceLoaded?.Invoke(this, new BoardArgs(null, _workPiece));
            }
        }
        #endregion WorkPiece

        public event EventHandler<BoardArgs> WorkPieceLoaded;
        public event EventHandler<BoardArgs> WorkPieceUnloaded;

    }
}
