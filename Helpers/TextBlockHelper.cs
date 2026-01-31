using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using SharpVectors.Converters;

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

                // ONLY split by symbols and newlines. 
                // We will handle italics manually by detecting ( and ).
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
                        // Handle the Symbol SVG
                        string symbol = token.Trim('{', '}').Replace("/", "");
                        string path = $"pack://application:,,,/ManaHub;component/Assets/Symbols/{symbol}.svg";

                        try
                        {
                            var container = new InlineUIContainer(new SvgViewbox
                            {
                                UriSource = new Uri(path),
                                Width = 11,
                                Height = 11,
                                Margin = new Thickness(1, 0, 1, -2)
                            });
                            textBlock.Inlines.Add(container);
                        }
                        catch
                        {
                            // If image fails, add text. Apply italics if we are mid-reminder text.
                            textBlock.Inlines.Add(new Run(token)
                            {
                                FontStyle = isInsideParentheses ? FontStyles.Italic : FontStyles.Normal
                            });
                        }
                    }
                    else
                    {
                        // This is a text block. It might contain '(' or ')'
                        // We need to handle the case where italics start or end mid-string.
                        ProcessTextWithItalics(textBlock, token, ref isInsideParentheses);
                    }
                }
            }
        }

        private static void ProcessTextWithItalics(TextBlock tb, string text, ref bool isInside)
        {
            // Split the text by ( and ) but keep the delimiters
            string[] parts = Regex.Split(text, @"([\(\)])");

            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part)) continue;

                if (part == "(") isInside = true;

                tb.Inlines.Add(new Run(part)
                {
                    FontStyle = isInside ? FontStyles.Italic : FontStyles.Normal
                });

                if (part == ")") isInside = false;
            }
        }
    }
}