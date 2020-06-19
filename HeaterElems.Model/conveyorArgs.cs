using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeaterElems.Model
{
    public class ConveyorArgs : EventArgs
    {
        public Conveyor Conveyor { get; private set; }

        public ConveyorArgs(Conveyor conveyor) {
            Conveyor = conveyor;
        }
    }
}
