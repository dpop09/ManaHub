using System.Globalization;
using System.Windows.Data;

namespace ManaHub.Converters
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colors = value as IEnumerable<string>;
            if (colors == null || !colors.Any())
                return "Colorless";

            string[] colorArray = colors.ToArray();

            if (colorArray.Length > 1)
                return "Multicolored";

            // map the single character to the full word
            return colorArray[0].ToUpperInvariant() switch
            {
                "W" => "White",
                "U" => "Blue",
                "B" => "Black",
                "R" => "Red",
                "G" => "Green",
                _ => "Colorless"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
