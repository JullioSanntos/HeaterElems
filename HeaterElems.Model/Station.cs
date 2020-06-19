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

        #region Board
        private Board _board;
        public Board Board {
            get { return _board; }
            set
            {
                var previousBoard = _board;
                SetProperty(ref _board, value);
                if (_board == null) BoardUnloaded?.Invoke(this, new BoardArgs(null, previousBoard));
                else BoardLoaded?.Invoke(this, new BoardArgs(null, _board));
            }
        }
        #endregion Board

        public event EventHandler<BoardArgs> BoardLoaded;
        public event EventHandler<BoardArgs> BoardUnloaded;

    }
}
