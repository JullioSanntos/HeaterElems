using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeaterElems.Model
{
    public class BoardArgs : EventArgs
    {
        public Station Station { get; private set; }
        public Board Board { get; private set; }

        public BoardArgs(Station station, Board board)
        {
            Station = station;
            Board = board;
        }
    }


}
