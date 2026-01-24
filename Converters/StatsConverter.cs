using System.Globalization;
using System.Windows.Data;

namespace ManaHub.Converters
{
    public class StatsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string power = values[0].ToString();
            string toughness = values[1].ToString();

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
