using System;
using System.Windows;
using System.Windows.Data;

namespace CarSpending
{
    public class MyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var pr = (bool)value;

            return pr ? Application.Current.FindResource("Wrench") : Application.Current.FindResource("GasStation");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
