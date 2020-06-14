using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;
using HeaterElems.Model;

namespace HeaterElems.Model 
{
    public class Heater : SetPropertyBase
    {

        #region StopWatch
        private StopWatch _stopWatch;
        public StopWatch StopWatch {
            get { return _stopWatch ?? (_stopWatch = new StopWatch()); }
            set { SetProperty(ref _stopWatch, value); }
        }
        #endregion StopWatch

	}
}
