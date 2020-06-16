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

        private ConveyorBelt Lane1 => ModelContext.ConveyorBets.First();
        private ConveyorBelt Lane2 => ModelContext.ConveyorBets.Skip(1).First();

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
            Lane1.BoardsMoved -= Lane1_BoardUnloaded;
            Lane1.BoardsMoved += Lane1_BoardUnloaded;
            Lane1StepRun();
            Lane2.BoardsMoved -= Lane2_BoardUnloaded;
            Lane2.BoardsMoved += Lane2_BoardUnloaded;
            Lane2StepRun();
        }

        

        public void Step()
        {
            Lane1StepRun();

            Lane2StepRun();
        }

        private void Lane1_BoardUnloaded(object sender, ConveyorArgs e)
        {
            Lane1StepRun();
        }


        private void Lane2_BoardUnloaded(object sender, ConveyorArgs e)
        {
            Lane2StepRun();
        }



        private void Lane1StepRun()
        {
            if (PreStation1.Board == null)
            {
                PreStation1.Board = new Board() {Id = CurrentBoardId += 1 };
                //PreStation1.Board.StopWatch.Start();
            }
        }

        private void Lane2StepRun()
        {
            if (PreStation2.Board == null)
            {
                PreStation2.Board = new Board() {Id = CurrentBoardId += 1 };
                //PreStation2.Board.StopWatch.Start();
            }
        }

    }
}
    
