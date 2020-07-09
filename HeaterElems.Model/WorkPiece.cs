using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;

namespace HeaterElems.Model
{
    public class WorkPiece 
    {
        #region Id
        public string Id { get; protected set; }
        #endregion Id

        #region ProgressiveTimer
        private ProgressiveTimer _progressiveTimer;
        public ProgressiveTimer ProgressiveTimer {
            get { return _progressiveTimer ?? (_progressiveTimer = new ProgressiveTimer()); }
            protected set { _progressiveTimer = value; }
        }
        #endregion ProgressiveTimer

        public WorkPiece(string id)
        {
            Id = id;
        }
    }
}
