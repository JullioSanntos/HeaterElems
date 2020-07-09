using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;

namespace HeaterElems.Model
{
    public class Station : SetPropertyBase
    {

        #region events
        public event EventHandler<WorkPiece> WorkPieceLoaded;
        public event EventHandler<WorkPiece> WorkPieceUnloaded;
        #endregion events

        #region DownstreamOrder
        private int _downstreamOrder;
        public int DownstreamOrder
        {
            get { return _downstreamOrder; }
            private set { SetProperty(ref _downstreamOrder, value); }
        }
        #endregion DownstreamOrder

        #region Name
        private string _name;
        public string Name
        {
            get { return _name ?? ($"{DownstreamOrder} - {StationType}"); }
            set { SetProperty(ref _name, value); }
        }
        #endregion Name

        #region Heater
        private Heater _heater;
        public Heater Heater {
            get { return _heater ?? (_heater = new Heater()); }
            set { SetProperty(ref _heater, value); }
        }
        #endregion Heater

        #region HasBoard
        public bool HasBoard { get { return WorkPiece != null; } }
        #endregion HasBoard

        #region WorkPiece
        private WorkPiece _workPiece;
        public WorkPiece WorkPiece {
            get { return _workPiece; }
            set
            {

                var prevWorkPiece = _workPiece;
                SetProperty(ref _workPiece, value);
                if (prevWorkPiece != null) WorkPieceUnloaded?.Invoke(this, prevWorkPiece);
                if (_workPiece != null) WorkPieceLoaded?.Invoke(this, _workPiece);
                RaisePropertyChanged(nameof(HasBoard));
            }
        }
        #endregion WorkPiece

        #region StationType
        private StationTypeEnum _stationType;
        public StationTypeEnum StationType
        {
            get { return _stationType; }
            protected set { SetProperty(ref _stationType, value); }
        }
        #endregion StationType

        #region constructors
        public Station(int downstreamOrder, StationTypeEnum stationType)
        {
            DownstreamOrder = downstreamOrder;
            StationType = stationType;
            //this.WorkPieceLoaded += async (s, e) => await StartUnloadTimer();
        }

        //private readonly Random _randomTime = new Random();
        //private readonly ProgressiveTimer _unloadTimer = new ProgressiveTimer();
        //private async Task StartUnloadTimer()
        //{
        //    var heatingTime = _randomTime.Next(100, 1300);
        //    _unloadTimer.StopAfter(heatingTime);
        //    await _unloadTimer.StartAsync();
        //    WorkPiece = null;


        //}
        #endregion constructors

    }
}
