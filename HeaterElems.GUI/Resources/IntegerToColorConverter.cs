using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace HeaterElems.GUI.Resources
{
    public class IntegerToColorConverter : IValueConverter
    {
        #region ColorsList
        private List<Color> _colorsList;

        public List<Color> ColorsList
        {
            get
            {
                if (_colorsList == null)
                {
                    _colorsList = new List<Color>() {
                        Colors.Beige, Colors.Aquamarine, Colors.GreenYellow, Colors.Yellow, Colors.DeepSkyBlue, Colors.LightSalmon, Colors.LightSlateGray
                    };

                    //Shuffle(_colorsList);
                }

                return _colorsList;
            }
            //set { _colorsList = value; }
        }
        #endregion ColorsList

        #region shuffle
        private static Random rng = new Random();

        public static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        #endregion shuffle

        public IntegerToColorConverter()
        {
            
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intValue = 0;

            if (value != null) Int32.TryParse(value.ToString(), out intValue);
            var cix = intValue % 6;

            var color = ColorsList[cix];
            return new SolidColorBrush(color);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
