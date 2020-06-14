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
            get { return _board ?? (_board = new Board()); }
            set { SetProperty(ref _board, value); }
        }
        #endregion Board
    }
}
