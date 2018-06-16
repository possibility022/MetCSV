using System;
using System.Globalization;
using System.Windows.Data;

namespace METCSV.WPF.Converters
{
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parsed = int.TryParse((string)value, out int v);
            if (parsed)
            {
                return v;
            }
            else
            {
                return new object(); // what we should return in this case?
            }
        }
    }
}
