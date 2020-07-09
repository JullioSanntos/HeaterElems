using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;

namespace HeaterElems.Model
{
    public class Station : SetPropertyBase
    {

        #region events
        public event EventHandler<BoardArgs> WorkPieceLoaded;
        public event EventHandler<BoardArgs> WorkPieceUnloaded;
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
            get { return _name; }
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

        #region WorkPiece
        private WorkPiece _workPiece;
        public WorkPiece WorkPiece {
            get { return _workPiece; }
            set
            {
                var previousBoard = _workPiece;
                SetProperty(ref _workPiece, value);
                //if (_workPiece == null) WorkPieceUnloaded?.Invoke(this, new BoardArgs(null, previousBoard));
                //else WorkPieceLoaded?.Invoke(this, new BoardArgs(null, _workPiece));
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
            this.PropertyChanged += Station_PropertyChanged;
        }

        private void Station_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(WorkPiece)) return;
            if (WorkPiece == null) WorkPieceUnloaded?.Invoke(this, null);
            else WorkPieceLoaded?.Invoke(this, new BoardArgs(WorkPiece));
        }
        #endregion constructors

    }
}
