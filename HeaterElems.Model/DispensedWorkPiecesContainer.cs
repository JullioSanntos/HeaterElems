using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;

namespace HeaterElems.Model
{
    public class DispensedWorkPiecesContainer : SetPropertyBase
    {
        #region DispensedBoards
        private ObservableCollection<WorkPiece> _dispensedBoards;
        public ObservableCollection<WorkPiece> DispensedBoards {
            get => _dispensedBoards ?? (_dispensedBoards = new ObservableCollection<WorkPiece>());
            set => SetProperty(ref _dispensedBoards, value);
        }
        #endregion DispensedBoards
    }
}
