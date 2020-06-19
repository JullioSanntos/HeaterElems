﻿using HeaterElems.Model;
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
        public RelayCommand StopCommand => new RelayCommand((o) => HasStopped = true);
        public RelayCommand StepCommand => new RelayCommand((o) => Step());

        private Conveyor Lane1 => ModelContext.Conveyors.First();
        private Conveyor Lane2 => ModelContext.Conveyors.Skip(1).First();

        private Station PreStation1 => Lane1.PreStation;
        private Station PreStation2 => Lane2.PreStation;
        private Station MainStation1 => Lane1.MainStation;
        private Station MainStation2 => Lane2.MainStation;
        private Station PostStation1 => Lane1.PostStation;
        private Station PostStation2 => Lane2.PostStation;

        public int CurrentBoardId = 0;

        #region HasStopped
        private bool _hasStopped;
        public bool HasStopped
        {
            get { return _hasStopped; }
            set { SetProperty(ref _hasStopped, value); }
        }
        #endregion HasStopped


        public void Run()
        {
            HasStopped = false;
            Lane1.BoardDispensed -= Lane1_BoardDispensed;
            Lane1.BoardDispensed += Lane1_BoardDispensed;
            Lane1.PreStation.BoardUnloaded -= PreStation1_BoardUnloaded;
            Lane1.PreStation.BoardUnloaded += PreStation1_BoardUnloaded;
            Lane1StepRun();
            Lane2.BoardDispensed -= Lane2_BoardDispensed;
            Lane2.BoardDispensed += Lane2_BoardDispensed;
            Lane2.PreStation.BoardUnloaded -= PreStation2_BoardUnloaded;
            Lane2.PreStation.BoardUnloaded += PreStation2_BoardUnloaded;
            Lane2StepRun();
        }

        private void PreStation1_BoardUnloaded(object sender, BoardArgs e)
        {
            Lane1StepRun();
        }

        private void PreStation2_BoardUnloaded(object sender, BoardArgs e)
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
            if (PreStation1.Board == null)
            {
                PreStation1.Board = new Board() {Id = CurrentBoardId += 1 };
                //PreStation1.Board.ProgressiveTimer.Start();
            }
        }

        private void Lane2StepRun()
        {
            if (PreStation2.Board == null)
            {
                PreStation2.Board = new Board() {Id = CurrentBoardId += 1 };
                //PreStation2.Board.ProgressiveTimer.Start();
            }
        }

    }
}
    
