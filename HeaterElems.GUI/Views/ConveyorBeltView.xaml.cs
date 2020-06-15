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
    /// Interaction logic for ConveyorBeltView.xaml
    /// </summary>
    public partial class ConveyorBeltView : UserControl
    {
        private ConveyorBeltViewModel _vm { get; set; }
        public ConveyorBeltView()
        {
            InitializeComponent();
            this.Loaded += ConveyorBeltView_Loaded;
            _vm = new ConveyorBeltViewModel();
        }

        private void ConveyorBeltView_Loaded(object sender, RoutedEventArgs e)
        {
            _vm.ModelContext = DataContext as ConveyorBelt;
            this.DataContext = _vm;
        }
    }
}
