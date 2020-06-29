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
    /// Interaction logic for StationView.xaml
    /// </summary>
    public partial class StationView : UserControl
    {
        private StationViewModel _vm { get; set; }

        public StationView()
        {
            InitializeComponent();
            //this.Loaded += StationView_Loaded;
            //_vm = new StationViewModel();
        }

        private void StationView_Loaded(object sender, RoutedEventArgs e)
        {
            _vm.ModelContext = DataContext as Station;
            this.DataContext = _vm;

        }
    }
}
