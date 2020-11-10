using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Component.Converter
{
    [ValueConversion(typeof(Boolean?),typeof(Style))]
    public class BooleanToHighlightStyleConverter : IValueConverter
    {
        public Style DefaultStyle
        {
            get;
            set;
        }

        public Style HighlightStyle
        {
            get;
            set;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Boolean?)value ?? false)
                return HighlightStyle;
            else
                return DefaultStyle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
