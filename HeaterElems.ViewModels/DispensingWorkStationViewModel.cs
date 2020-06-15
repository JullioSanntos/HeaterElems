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

        private ConveyorBelt Lane1 => ModelContext.ConveyorBets.First();
        private ConveyorBelt Lane2 => ModelContext.ConveyorBets.Skip(1).First();

        private Station PreStation1 => Lane1.PreStation;
        private Station PreStation2 => Lane2.PreStation;
        private Station MainStation1 => Lane1.MainStation;
        private Station MainStation2 => Lane2.MainStation;
        private Station PostStation1 => Lane1.PostStation;
        private Station PostStation2 => Lane2.PostStation;

        public int CurrentBoardId = 1;


        public void Run() {

        }

        public void Step()
        {

            if (PreStation1.Board == null)
            {
                PreStation1.Board = new Board() { Id = CurrentBoardId++ };
                //PreStation1.Board.StopWatch.Start();
            }

            if (PreStation2.Board == null)
            {
                PreStation2.Board = new Board() { Id = CurrentBoardId++ };
                //PreStation2.Board.StopWatch.Start();
            }
        }

 
    }
}
    
