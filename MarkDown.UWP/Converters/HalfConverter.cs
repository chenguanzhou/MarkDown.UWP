using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MarkDown.UWP.Converters
{
    public class HalfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (double)value / 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (double)value * 2;
        }
    }
}
