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
                // Find everything between { }
                var matches = Regex.Matches(manaCost, @"\{([^}]+)\}");

                // Convert each symbol (like "W") into a Pack URI path
                return matches.Cast<Match>().Select(m =>
                {   
                    // Clean the symbol to remove the slash
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
