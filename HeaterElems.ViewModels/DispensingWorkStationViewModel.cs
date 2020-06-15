using HeaterElems.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;

namespace HeaterElems.ViewModels
{
    public class DispensingWorkStationViewModel : SetPropertyBase
    {
        public DispensingWorkStation ModelContext => DispensingWorkStation.Instance;


        public RelayCommand RunCommand => new RelayCommand((o) => Run());
        public RelayCommand StepCommand => new RelayCommand((o) => Step());


        public void Run() {

        }

        public int CurrentBoardId = 1;

        public void Step() {
            var preStation1 = ModelContext.ConveyorBets.First().PreStation;
            if (preStation1.Board == null) {
                preStation1.Board = new Board() {Id = CurrentBoardId++};
                return;
            }

            var preStation2 = ModelContext.ConveyorBets.Skip(1).First().PreStation;
            if (preStation2.Board == null) {
                preStation2.Board = new Board() { Id = CurrentBoardId++ };
                return;
            }
        }


 
    }
}
