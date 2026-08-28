using ManaHub.Models;
using ManaHub.ViewModels;
using System.Globalization;
using System.Windows.Data;

using ManaHub.Domain;

namespace ManaHub.Converters
{
    class CardToInteractiveCardViewModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Card card)
            {
                return new InteractiveCardViewModel(card);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
