using System.Globalization;
using System.Windows.Data;

namespace METCSV.WPF.Converters
{
    public class DoubleToPercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value * 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var successful = double.TryParse((string)value, out double val);
            if (successful)
            {
                return val / 100;
            }
            else
            {
                return value;
            }
        }
    }
}
