using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;
using HeaterElems.Model;

namespace HeaterElems.ViewModels
{
    public class StationViewModel : ViewModelBase<Station>
    {
        
        #region HasBoard
        public bool HasBoard {
            get { return WorkPieceViewModel?.ModelContext != null; }
        }
        #endregion HasBoard

        #region WorkPieceViewModel
        private WorkPieceViewModel _workPieceViewModel;

        public WorkPieceViewModel WorkPieceViewModel
        {
            get { return _workPieceViewModel; }
            set { SetProperty(ref _workPieceViewModel, value); }
        }
        #endregion WorkPieceViewModel

        public StationViewModel() {
            this.PropertyChanged += StationViewModel_PropertyChanged;
        }

        private void StationViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(WorkPieceViewModel):
                    RaisePropertyChanged(nameof(HasBoard));
                    WorkPieceViewModel.PropertyChanged -= WorkPieceViewModel_PropertyChanged;
                    WorkPieceViewModel.PropertyChanged += WorkPieceViewModel_PropertyChanged;
                    break;
            }
        }


        private void WorkPieceViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WorkPieceViewModel.ModelContext)) RaisePropertyChanged(nameof(HasBoard));
        }

        public void LoadBoard(int boardId)
        {
            ModelContext.WorkPiece = new WorkPiece(boardId.ToString());
            WorkPieceViewModel = new WorkPieceViewModel {ModelContext = ModelContext};
        }
    }
}
