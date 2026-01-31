using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ManaHub.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If the string is null, empty, or just a dash, hide the element
            string text = value as string;
            if (string.IsNullOrWhiteSpace(text) || text == "-")
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}