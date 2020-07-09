using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeaterElems.Model
{
    public class BoardArgs : EventArgs
    {
        public WorkPiece WorkPiece { get; private set; }

        public BoardArgs(WorkPiece workPiece)
        {
            WorkPiece = workPiece;
        }
    }


}
