using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace ManaHub.Converters
{
    internal class SymbolToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string manaCost && !string.IsNullOrWhiteSpace(manaCost))
            {
                // Find everything between { }
                var matches = Regex.Matches(manaCost, @"\{([^}]+)\}");

                // Convert each symbol (like "W") into a Pack URI path
                return matches.Cast<Match>().Select(m =>
                {   
                    // Clean the symbol to remove the slash
                    string symbol = m.Groups[1].Value.Replace("/", "");
                    // Return the absolute path of the mana symbol's correlating svg file
                    return $"pack://application:,,,/ManaHub;component/Assets/Symbols/{symbol}.svg";
                }).ToList();
            }
            return new List<string>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
