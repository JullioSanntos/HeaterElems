using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeaterElems.Common;

namespace HeaterElems.Model
{
    public class DispensserWorkStation : SetPropertyBase
    {
        #region ConveyorBets
        private ObservableCollection<ConveyorBet> _conveyorBets;
        public ObservableCollection<ConveyorBet> ConveyorBets {
            get => _conveyorBets;
            set => SetProperty(ref _conveyorBets, value);
        }
        #endregion ConveyorBets

        #region DispensedBoardsContainer
        private ObservableCollection<DispensedBoardsContainer> _dispensedBoardsContainer;
        public ObservableCollection<DispensedBoardsContainer> DispensedBoardsContainer {
            get => _dispensedBoardsContainer ?? (_dispensedBoardsContainer = new ObservableCollection<DispensedBoardsContainer>());
            set => SetProperty(ref _dispensedBoardsContainer, value);
        }
        #endregion DispensedBoardsContainer
    }
}
