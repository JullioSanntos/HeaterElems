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
        
        #region WorkPieceViewModel
        private WorkPieceViewModel _workPieceViewModel;

        public WorkPieceViewModel WorkPieceViewModel
        {
            get { return _workPieceViewModel; }
            set { SetProperty(ref _workPieceViewModel, value); }
        }
        #endregion WorkPieceViewModel

        public async Task LoadBoardAsync(int boardId)
        {
            ModelContext.WorkPiece = new WorkPiece(boardId.ToString());
            WorkPieceViewModel = new WorkPieceViewModel {ModelContext = ModelContext}; // use the same model context to facilitate access to Station.HasBoard property
            await StartUnloadTimer();
        }

        private readonly Random _randomTime = new Random();
        private readonly ProgressiveTimer _unloadTimer = new ProgressiveTimer();
        private async Task StartUnloadTimer()
        {
            var heatingTime = _randomTime.Next(2000, 2500);
            _unloadTimer.StopAfter(heatingTime);
            await _unloadTimer.StartAsync();
            ModelContext.WorkPiece = null;


        }
    }
}
