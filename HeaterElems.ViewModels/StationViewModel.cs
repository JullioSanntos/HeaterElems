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
        #region properties
        #region WorkPieceViewModel
        private WorkPieceViewModel _workPieceViewModel;

        public WorkPieceViewModel WorkPieceViewModel
        {
            get { return _workPieceViewModel; }
            set { SetProperty(ref _workPieceViewModel, value); }
        }
        #endregion WorkPieceViewModel

        #region WorkPiece
        private WorkPiece _workPiece;

        public WorkPiece WorkPiece {
            get { return _workPiece ?? (ModelContext.WorkPiece); }
            set { SetProperty(ref _workPiece, value); }
        }
        #endregion WorkPiece
        #endregion properties

        #region Constructors
        public StationViewModel() {
            this.PropertyChanged += StationViewModel_PropertyChanged;
 
        }

        private void StationViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ModelContext)) return;
            if (ModelContext == null) return;

            this.ModelContext.PropertyChanged += (s, st) => {
                if (st.PropertyName == nameof(ModelContext.WorkPiece)) RaisePropertyChanged(nameof(WorkPiece));
            };
        }
        #endregion Constructors

        public async Task LoadBoardAsync(WorkPiece workPiece)
        {
            ModelContext.WorkPiece = workPiece;
            WorkPieceViewModel = new WorkPieceViewModel {ModelContext = ModelContext}; // use the same model context to facilitate access to Station.HasBoard property
            await StartUnloadTimer();
        }

        private readonly Random _randomTime = new Random();
        private readonly ProgressiveTimer _unloadTimer = new ProgressiveTimer();
        private async Task StartUnloadTimer()
        {
            var heatingTime = _randomTime.Next(500, 1300);
            _unloadTimer.StopAfter(heatingTime);
            await _unloadTimer.StartAsync();
            ModelContext.WorkPiece = null;


        }
    }
}
