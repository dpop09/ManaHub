using System.Windows.Input;

using ManaHub.Contracts;
using ManaHub.Domain;
using ManaHub.MVVMs;

namespace ManaHub.ViewModels
{
    internal sealed class InteractiveCardViewModel : ViewModelBase
    {
        private readonly ICardImageService _images;
        private bool _isFlipped;

        public InteractiveCardViewModel(Card card, ICardImageService images)
        {
            InteractiveCard = card;
            _images = images;
            FlipCardCommand = new RelayCommand(_ => IsFlipped = !IsFlipped);
        }

        public Card InteractiveCard { get; }

        public bool IsFlipped
        {
            get => _isFlipped;
            set
            {
                if (_isFlipped == value)
                    return;

                _isFlipped = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(InteractiveCardImageUri));
            }
        }

        public Uri? InteractiveCardImageUri
        {
            get
            {
                string? imageUrl = IsFlipped
                    ? InteractiveCard.Back?.ImageUrl
                    : InteractiveCard.Front.ImageUrl;
                if (string.IsNullOrEmpty(imageUrl))
                    return null;

                return _images.GetImagePath(
                    $"{InteractiveCard.Id}_{(IsFlipped ? "back" : "front")}",
                    imageUrl,
                    () => OnPropertyChanged(nameof(InteractiveCardImageUri)));
            }
        }

        public ICommand FlipCardCommand { get; }
    }
}
