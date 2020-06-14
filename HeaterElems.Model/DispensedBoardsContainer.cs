using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;

namespace HeaterElems.Model
{
    public class DispensedBoardsContainer : SetPropertyBase
    {
        #region DispensedBoards
        private ObservableCollection<Board> _dispensedBoards;
        public ObservableCollection<Board> DispensedBoards {
            get => _dispensedBoards ?? (_dispensedBoards = new ObservableCollection<Board>());
            set => SetProperty(ref _dispensedBoards, value);
        }
        #endregion DispensedBoards
    }
}
