using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Component.Converter
{
    [ValueConversion(typeof(int), typeof(Style))]
    public class IntegerToStyleConverter : IValueConverter
    {

        public Style[] Styles
        {
            get;
            set;
        }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Styles == null)
                return null;
            var len = Styles.Length;
            var @int = (int)value;
            if (len <= 0 || !(0 <= @int && @int < len))
                return null;
            return Styles[@int];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
