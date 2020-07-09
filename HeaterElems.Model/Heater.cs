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
        #region IsPowered
        private bool _isPowered;
        public bool IsPowered {
            get { return _isPowered; }
            set { SetProperty(ref _isPowered, value); }
        }
        #endregion IsPowered
	}
}
