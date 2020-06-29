using HeaterElems.Model;
using HeaterElems.ViewModels;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HeaterElems.GUI.Resources
{
    public class ViewModelLocatorBehavior : Behavior<FrameworkElement>
    {
        private StationViewModel _vm { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            var frameworkElem = AssociatedObject as FrameworkElement;
            frameworkElem.Loaded += FrameworkElem_Loaded;
            _vm = new StationViewModel();
        }

        private void FrameworkElem_Loaded(object sender, RoutedEventArgs e)
        {
            var frameworkElem = sender as FrameworkElement;
            if (frameworkElem == null) return;
            _vm.ModelContext = frameworkElem.DataContext as Station;
            frameworkElem.DataContext = _vm;
        }



        //private Point elementStartPosition;
        //private Point mouseStartPosition;
        //private TranslateTransform transform = new TranslateTransform();

        //protected override void OnAttached()
        //{
        //    Window parent = Application.Current.MainWindow;
        //    AssociatedObject.RenderTransform = transform;

        //    AssociatedObject.MouseLeftButtonDown += (sender, e) =>
        //    {
        //        elementStartPosition = AssociatedObject.TranslatePoint(new Point(), parent);
        //        mouseStartPosition = e.GetPosition(parent);
        //        AssociatedObject.CaptureMouse();
        //    };

        //    AssociatedObject.MouseLeftButtonUp += (sender, e) =>
        //    {
        //        AssociatedObject.ReleaseMouseCapture();
        //    };

        //    AssociatedObject.MouseMove += (sender, e) =>
        //    {
        //        Vector diff = e.GetPosition(parent) - mouseStartPosition;
        //        if (AssociatedObject.IsMouseCaptured)
        //        {
        //            transform.X = diff.X;
        //            transform.Y = diff.Y;
        //        }
        //    };
        //}
    }
}
