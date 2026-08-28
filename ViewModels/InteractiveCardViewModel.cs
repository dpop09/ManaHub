using ManaHub.Models;
using ManaHub.MVVMs;
using ManaHub.Services;
using System.Windows.Input;

using ManaHub.Domain;

namespace ManaHub.ViewModels
{
    internal class InteractiveCardViewModel : ViewModelBase
    {
        private Card _interactiveCard;
        public Card InteractiveCard 
        {
            get => _interactiveCard;
            set
            {
                _interactiveCard = value;
                IsFlipped = false;
                OnPropertyChanged();
                OnPropertyChanged(nameof(InteractiveCardImageUri));
            }
        }
        private bool _isFlipped;
        public bool IsFlipped 
        {
            get => _isFlipped;
            set
            {
                _isFlipped = value;
                OnPropertyChanged();
            }
        }
        public Uri InteractiveCardImageUri 
        {
            get
            {
                if (InteractiveCard == null) return null;

                string? urlToUse = IsFlipped ? InteractiveCard.Back?.ImageUrl : InteractiveCard.Front.ImageUrl;

                if (string.IsNullOrEmpty(urlToUse))
                    return null;

                return CardImageService.GetImagePath(
                    $"{InteractiveCard.Id}_{(IsFlipped ? "back" : "front")}", 
                    urlToUse, 
                    () => OnPropertyChanged(nameof(InteractiveCardImageUri))
                );
            }
        }
        public ICommand FlipCardCommand { get; }

        public InteractiveCardViewModel(Card card) 
        {
            _interactiveCard = card;
            FlipCardCommand = new RelayCommand(o => IsFlipped = !IsFlipped);
        }
    }
}
