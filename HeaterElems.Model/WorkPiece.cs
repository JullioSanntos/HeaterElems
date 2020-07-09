using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;

namespace HeaterElems.Model
{
    public class WorkPiece :SetPropertyBase
    {
        #region Id
        private string _id;

        public string Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }
        #endregion Id

        #region ProgressiveTimer
        private ProgressiveTimer _progressiveTimer;
        public ProgressiveTimer ProgressiveTimer {
            get { return _progressiveTimer ?? (ProgressiveTimer = new ProgressiveTimer()); }
            protected set { _progressiveTimer = value; }
        }
        #endregion ProgressiveTimer

        public WorkPiece(string id)
        {
            Id = id;
        }
    }
}
