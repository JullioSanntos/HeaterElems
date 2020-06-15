using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeaterElems.Model
{
    public class BoardArgs : EventArgs
    {
        public Board Board { get; private set; }

        public BoardArgs(Board board) {
            Board = board;
        }
    }


}
