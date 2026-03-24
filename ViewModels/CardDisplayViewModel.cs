using ManaHub.Models;
using ManaHub.MVVMs;
using ManaHub.Services;
using System.Windows.Input;

namespace ManaHub.ViewModels
{
    class CardDisplayViewModel : ViewModelBase
    {
        private readonly DeckEditorPageViewModel _depvm;
        private Card _cardDisplay;
        public Card CardDisplay 
        {
            get => _cardDisplay;
            set 
            { 
                _cardDisplay = value;
                IsFlipped = false;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayImageUri));
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
        public Uri DisplayImageUri
        {
            get
            {
                if (CardDisplay == null) return null;

                // Determine which URL to use from the model
                string urlToUse = IsFlipped ? CardDisplay.SecondaryImageUrl : CardDisplay.PrimaryImageUrl;

                // Fallback: If the card isn't flipped but has no Primary, or is flipped but has no Secondary
                if (string.IsNullOrEmpty(urlToUse))
                    return null;

                return CardImageService.GetImagePath(
                    $"{CardDisplay.Id}_{(IsFlipped ? "back" : "front")}", // Cache front and back separately!
                    urlToUse,
                    () => OnPropertyChanged(nameof(DisplayImageUri))
                );
            }
        }
        public ICommand FlipCardCommand { get; }
        

        public CardDisplayViewModel(DeckEditorPageViewModel depvm)
        {
            _depvm = depvm;
            FlipCardCommand = new RelayCommand(o => IsFlipped = !IsFlipped);
        }
    }
}
