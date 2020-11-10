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
    [ValueConversion(typeof(Boolean?),typeof(BitmapImage))]
    public class BooleanToPathConverter : IValueConverter
    {
        public string PathForTrue { get; set; }
        public string PathForFalse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Boolean?)value ?? false)
                return new BitmapImage(new Uri(PathForTrue, UriKind.RelativeOrAbsolute));
            else
                return new BitmapImage(new Uri(PathForFalse, UriKind.RelativeOrAbsolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
