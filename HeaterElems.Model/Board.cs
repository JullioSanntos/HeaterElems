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

        #region StopWatch
        private StopWatch _stopWatch;
        public StopWatch StopWatch {
            get { return _stopWatch ?? (_stopWatch = new StopWatch()); }
            set { SetProperty(ref _stopWatch, value); }
        }
        #endregion StopWatch
    }
}
