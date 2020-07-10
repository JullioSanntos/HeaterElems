using HeaterElems.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;

namespace HeaterElems.ViewModels
{
    public class DispensingWorkStationViewModel : SetPropertyBase
    {
        #region properties
        public DispensingWorkStation ModelContext => DispensingWorkStation.Instance;

        public RelayCommand RunCommand => new RelayCommand((o) => Run());
        public RelayCommand StopCommand => new RelayCommand((o) => HasStopped = true);
        public RelayCommand StepCommand => new RelayCommand((o) => Step());

        public int CurrentBoardId = 0;

        #region HasStopped
        private bool _hasStopped;
        public bool HasStopped
        {
            get { return _hasStopped; }
            set { SetProperty(ref _hasStopped, value); }
        }
        #endregion HasStopped

        #endregion properties

        #region constructor
        public DispensingWorkStationViewModel()
        {

        }




        #endregion constructor

        public void Run() { HasStopped = false; }

        private void PreStation1WorkPieceUnloaded(object sender, BoardArgs e) { Lane1StepRun(); }

        private void PreStation2WorkPieceUnloaded(object sender, BoardArgs e)
        {
            Lane2StepRun();
        }

        public void Step()
        {
            Lane1StepRun();

            Lane2StepRun();
        }

        private void Lane1_BoardDispensed(object sender, BoardArgs e)
        {
            Lane1StepRun();
        }


        private void Lane2_BoardDispensed(object sender, BoardArgs e)
        {
            Lane2StepRun();
        }



        private void Lane1StepRun()
        {
            //if (PreStation1.WorkPiece == null)
            //{
            //    PreStation1.WorkPiece = new WorkPiece((CurrentBoardId += 1).ToString());
            //    //PreStation1.WorkPiece.ProgressiveTimer.Start();
            //}
        }

        private void Lane2StepRun()
        {
            //if (PreStation2.WorkPiece == null)
            //{
            //    PreStation2.WorkPiece = new WorkPiece((CurrentBoardId += 1).ToString());
            //    //PreStation2.WorkPiece.ProgressiveTimer.Start();
            //}
        }

    }
}
    
