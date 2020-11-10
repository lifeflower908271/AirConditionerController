using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Component.Converter
{
    [ValueConversion(typeof(Boolean?),typeof(String))]
    public class BooleanToStringConverter : IValueConverter
    {
        public string StringForTrue { get; set; }
        public string StringForFalse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Boolean?)value ?? false)
                return StringForTrue;
            else
                return StringForFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
