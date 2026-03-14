using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;

namespace ManaHub.Converters
{
    internal class SymbolToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string manaCost && !string.IsNullOrWhiteSpace(manaCost))
            {
                // Matches either {symbol} OR //
                // Group 1: The content inside { }
                // Group 2: The double slash //
                var matches = Regex.Matches(manaCost, @"\{([^}]+)\}|(//)");

                return matches.Cast<Match>().Select(m =>
                {
                    // If it's a double slash, return our special token
                    if (m.Groups[2].Success)
                    {
                        return "SEPARATOR";
                    }

                    // It's a standard symbol
                    string symbol = m.Groups[1].Value.Replace("/", "");
                    string primaryPath = $"pack://application:,,,/ManaHub;component/Assets/Symbols/{symbol}.svg";
                    const string fallbackPath = "pack://application:,,,/ManaHub;component/Assets/Symbols/fallback.svg";

                    try
                    {
                        var uri = new Uri(primaryPath);
                        var resource = Application.GetResourceStream(uri);
                        if (resource != null)
                            return primaryPath;
                    }
                    catch
                    {
                        System.Diagnostics.Debug.WriteLine($"Mana Symbol Missing: {symbol}. Using placeholder.");
                    }

                    // Your dedicated question mark svg placeholder is returned here
                    return fallbackPath;
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
