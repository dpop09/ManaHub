using System.Globalization;
using System.Windows.Data;

namespace ManaHub.Converters
{
    public class StatsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string power = values.ElementAtOrDefault(0)?.ToString() ?? string.Empty;
            string toughness = values.ElementAtOrDefault(1)?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(power) && string.IsNullOrEmpty(toughness))
                return "-";

            return $"{power}/{toughness}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
