using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;
using HeaterElems.Model;

namespace HeaterElems.ViewModels
{
    public class DispensedWorkPiecesContainerViewModel : ViewModelBase<DispensedWorkPiecesContainer>
    {
        #region DispensedBoardsVmList
        private ObservableCollection<WorkPieceViewModel> _dispensedBoardsVmList;

        public ObservableCollection<WorkPieceViewModel> DispensedBoardsVmList
        {
            get { return _dispensedBoardsVmList ?? (DispensedBoardsVmList = new ObservableCollection<WorkPieceViewModel>()); }
            set
            {
                SetProperty(ref _dispensedBoardsVmList, value);
                _dispensedBoardsVmList.CollectionChanged += DispensedBoardsVmListCollectionChanged;
            }
        }

        private void DispensedBoardsVmListCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var listOfNew = e.NewItems.Cast<WorkPieceViewModel>();
                    listOfNew.ToList().ForEach(wpvm => ModelContext.DispensedBoards.Add(wpvm.WorkPiece));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var listOfOld = e.OldItems.Cast<WorkPieceViewModel>();
                    listOfOld.ToList().ForEach(wpvm => ModelContext.DispensedBoards.Add(wpvm.WorkPiece));
                    break;
                
            }
        }
        #endregion DispensedBoardsVmList
    }
}
