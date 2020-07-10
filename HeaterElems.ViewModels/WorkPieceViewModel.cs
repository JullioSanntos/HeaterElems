using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;
using HeaterElems.Model;

namespace HeaterElems.ViewModels
{
    public class WorkPieceViewModel: ViewModelBase<Station>
    {
        #region WorkPiece
        private WorkPiece _workPiece;

        public WorkPiece WorkPiece
        {
            get { return _workPiece ?? (ModelContext.WorkPiece); }
            set { SetProperty(ref _workPiece, value); }
        }
        #endregion WorkPiece
    }
}
