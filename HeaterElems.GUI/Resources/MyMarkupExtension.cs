using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace HeaterElems.GUI.Resources
{
    public class MyMarkupExtension : MarkupExtension
    {
        public string FirstStr { get; set; }
        public string SecondStr { get; set; }

        public Type VMType { get; set; }
        #region MarkupExtension
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var asef = (IXamlTypeResolver)serviceProvider.GetService(typeof(IXamlTypeResolver));

            var pvt = serviceProvider as IProvideValueTarget;
            if (pvt == null)
            {
                return FirstStr + " " + SecondStr;
            }


            var frameworkElement = pvt.TargetObject as ContentControl;
            if (frameworkElement == null)
            {
                return frameworkElement.DataContext; 
                return this;
            }
            //.... Code will run once the markup is correctly loaded
            var dataContext = frameworkElement.DataContext;
            return FirstStr + " " + SecondStr;
        }
    }
        #endregion MarkupExtension

}
