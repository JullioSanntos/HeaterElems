using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace HeaterElems.GUI.Resources
{
    public class MyMarkupExtension : MarkupExtension
    {
        public string FirstStr { get; set; }
        public string SecondStr { get; set; }
        #region MarkupExtension
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var asef = (IXamlTypeResolver)serviceProvider.GetService(typeof(IXamlTypeResolver));
            var asdf = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            return FirstStr + " " + SecondStr;
        }
    }
        #endregion MarkupExtension

}
