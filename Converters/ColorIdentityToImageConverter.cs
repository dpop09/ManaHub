using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ManaHub.Converters
{
    internal class ColorIdentityToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<string> symbols = new List<string>();
            if (value is IEnumerable<string> colors)
            {
                foreach (string color in colors)
                {
                    string primaryPath = $"pack://application:,,,/ManaHub;component/Assets/Symbols/{color.Trim()}.svg";
                    // Using your dedicated question mark placeholder as the fallback
                    const string fallbackPath = "pack://application:,,,/ManaHub;component/Assets/Symbols/question_mark.svg";

                    try
                    {
                        var uri = new Uri(primaryPath);
                        // Check if the resource actually exists in the assembly
                        var resource = Application.GetResourceStream(uri);

                        if (resource != null)
                            symbols.Add(primaryPath);
                        else
                            symbols.Add(fallbackPath);
                    }
                    catch
                    {
                        System.Diagnostics.Debug.WriteLine($"Mana Symbol Missing: {color}. Using question mark placeholder.");
                        symbols.Add(fallbackPath);
                    }
                }
                return symbols;
            }

            return symbols; // Returns empty list if no value provided
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
