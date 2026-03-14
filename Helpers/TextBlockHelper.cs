using SharpVectors.Converters;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ManaHub.Helpers
{
    public static class TextBlockHelper
    {
        public static readonly DependencyProperty FormattedTextProperty =
            DependencyProperty.RegisterAttached("FormattedText", typeof(string), typeof(TextBlockHelper),
            new PropertyMetadata(string.Empty, OnFormattedTextChanged));

        public static string GetFormattedText(DependencyObject obj) => (string)obj.GetValue(FormattedTextProperty);
        public static void SetFormattedText(DependencyObject obj, string value) => obj.SetValue(FormattedTextProperty, value);

        private static void OnFormattedTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBlock textBlock)
            {
                var text = e.NewValue as string;
                textBlock.Inlines.Clear();
                if (string.IsNullOrEmpty(text)) return;

                var tokens = Regex.Split(text, @"(\{.+?\}|\n)");
                bool isInsideParentheses = false;

                foreach (var token in tokens)
                {
                    if (string.IsNullOrEmpty(token)) continue;

                    if (token == "\n")
                    {
                        textBlock.Inlines.Add(new LineBreak());
                    }
                    else if (token.StartsWith("{") && token.EndsWith("}"))
                    {
                        // Handle Symbol SVG...
                        string symbol = token.Trim('{', '}').Replace("/", "");
                        string path = $"pack://application:,,,/ManaHub;component/Assets/Symbols/{symbol}.svg";

                        // Personalization: If file is missing, we use your question mark placeholder logic
                        string fallbackPath = "pack://application:,,,/ManaHub;component/Assets/Symbols/question.svg";

                        try
                        {
                            // Try to load the symbol
                            textBlock.Inlines.Add(CreateSvgInline(path));
                        }
                        catch
                        {
                            try
                            {
                                // Attempt fallback to your dedicated question mark SVG
                                textBlock.Inlines.Add(CreateSvgInline(fallbackPath));
                            }
                            catch
                            {
                                // Absolute fallback to text if even the question mark is missing
                                textBlock.Inlines.Add(new Run(token)
                                {
                                    FontStyle = isInsideParentheses ? FontStyles.Italic : FontStyles.Normal,
                                    FontWeight = isInsideParentheses ? FontWeights.Normal : FontWeights.Bold
                                });
                            }
                        }
                    }
                    else
                    {
                        ProcessTextWithItalics(textBlock, token, ref isInsideParentheses);
                    }
                }
            }
        }

        private static void ProcessTextWithItalics(TextBlock tb, string text, ref bool isInside)
        {
            string[] parts = Regex.Split(text, @"([\(\)])");

            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part)) continue;

                if (part == "(") isInside = true;

                tb.Inlines.Add(new Run(part)
                {
                    // Reminder text: Italics + Regular. Main text: Normal + Bold.
                    FontStyle = isInside ? FontStyles.Italic : FontStyles.Normal,
                    FontWeight = isInside ? FontWeights.Normal : FontWeights.Bold
                });

                if (part == ")") isInside = false;
            }
        }

        // Helper to keep the try/catch clean
        private static InlineUIContainer CreateSvgInline(string path)
        {
            return new InlineUIContainer(new SvgViewbox
            {
                UriSource = new Uri(path),
                Width = 11,
                Height = 11,
                Margin = new Thickness(1, 0, 1, -2)
            });
        }
    }
}