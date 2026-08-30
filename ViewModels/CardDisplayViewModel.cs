using System.Windows.Input;

using ManaHub.Contracts;
using ManaHub.Domain;
using ManaHub.MVVMs;

namespace ManaHub.ViewModels
{
    internal sealed class CardDisplayViewModel : ViewModelBase
    {
        private readonly ICardImageService _images;
        private Card? _card;
        private bool _isFlipped;

        public CardDisplayViewModel(ICardImageService images)
        {
            _images = images;
            FlipCardCommand = new RelayCommand(_ => IsFlipped = !IsFlipped);
        }

        public Card? CardDisplay
        {
            get => _card;
            set
            {
                if (ReferenceEquals(_card, value))
                    return;

                _card = value;
                IsFlipped = false;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayImageUri));
            }
        }

        public bool IsFlipped
        {
            get => _isFlipped;
            set
            {
                if (_isFlipped == value)
                    return;

                _isFlipped = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayImageUri));
            }
        }

        public Uri? DisplayImageUri
        {
            get
            {
                if (CardDisplay == null)
                    return null;

                string? imageUrl = IsFlipped ? CardDisplay.Back?.ImageUrl : CardDisplay.Front.ImageUrl;
                if (string.IsNullOrEmpty(imageUrl))
                    return null;

                return _images.GetImagePath(
                    $"{CardDisplay.Id}_{(IsFlipped ? "back" : "front")}",
                    imageUrl,
                    () => OnPropertyChanged(nameof(DisplayImageUri)));
            }
        }

        public ICommand FlipCardCommand { get; }
    }
}
