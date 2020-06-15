using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HeaterElems.Model;
using HeaterElems.ViewModels;

namespace HeaterElems.GUI.Views
{
    /// <summary>
    /// Interaction logic for BorderView.xaml
    /// </summary>
    public partial class BoardView : UserControl
    {
        private BoardViewModel _vm { get; set; }

        public BoardView()
        {
            InitializeComponent();
            this.Loaded += BoardView_Loaded;
            _vm = new BoardViewModel();
        }

        private void BoardView_Loaded(object sender, RoutedEventArgs e)
        {
            _vm.ModelContext = DataContext as Board;
            this.DataContext = _vm;
        }
    }
}
