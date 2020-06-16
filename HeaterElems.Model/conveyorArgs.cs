using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeaterElems.Model
{
    public class ConveyorArgs : EventArgs
    {
        public ConveyorBelt Conveyor { get; private set; }

        public ConveyorArgs(ConveyorBelt conveyor) {
            Conveyor = conveyor;
        }
    }
}
