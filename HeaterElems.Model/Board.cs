using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;

namespace HeaterElems.Model
{
    public class Board : SetPropertyBase
    {
        #region Id
        private int _id;
        public int Id {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        #endregion Id

        #region ProgressiveTimer
        private ProgressiveTimer _progressiveTimer;
        public ProgressiveTimer ProgressiveTimer {
            get { return _progressiveTimer ?? (_progressiveTimer = new ProgressiveTimer()); }
            set { SetProperty(ref _progressiveTimer, value); }
        }
        #endregion ProgressiveTimer
    }
}
