using System.Globalization;
using System.Windows.Data;

namespace ManaHub.Converters
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string colorString = value as string;

            if (string.IsNullOrWhiteSpace(colorString))
                return "Colorless";

            string[] colors = colorString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (colors.Length > 1)
                return "Multicolored";

            // map the single character to the full word
            return colors[0].ToUpper() switch
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
