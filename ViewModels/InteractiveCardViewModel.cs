using System.Windows.Input;

using ManaHub.Contracts;
using ManaHub.Domain;
using ManaHub.MVVMs;

namespace ManaHub.ViewModels
{
    internal sealed class InteractiveCardViewModel : ViewModelBase, IDisposable
    {
        private bool _isFlipped;
        private bool _isActive;

        public InteractiveCardViewModel(Card card, ICardImageService images)
        {
            InteractiveCard = card;
            Image = new AsyncImageViewModel(images);
            FlipCardCommand = new RelayCommand(_ => Flip());
        }

        public Card InteractiveCard { get; }
        public AsyncImageViewModel Image { get; }
        public ICommand FlipCardCommand { get; }

        public bool IsFlipped
        {
            get => _isFlipped;
            private set
            {
                if (_isFlipped == value)
                    return;

                _isFlipped = value;
                OnPropertyChanged();
            }
        }

        public Task ActivateAsync(CancellationToken cancellationToken = default)
        {
            _isActive = true;
            return LoadCurrentImageAsync(cancellationToken);
        }

        public void Deactivate()
        {
            _isActive = false;
            Image.Clear();
        }

        public void Dispose()
        {
            Deactivate();
            Image.Dispose();
        }

        private void Flip()
        {
            IsFlipped = !IsFlipped;
            if (_isActive)
                StartCurrentImageLoad();
        }

        private void StartCurrentImageLoad()
        {
            string? imageUrl = GetCurrentImageUrl();
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                Image.Clear();
                return;
            }

            string cacheKey = $"{InteractiveCard.Id}_{(IsFlipped ? "back" : "front")}";
            Image.StartLoad(cacheKey, imageUrl);
        }

        private Task LoadCurrentImageAsync(CancellationToken cancellationToken)
        {
            string? imageUrl = GetCurrentImageUrl();
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                Image.Clear();
                return Task.CompletedTask;
            }

            string cacheKey = $"{InteractiveCard.Id}_{(IsFlipped ? "back" : "front")}";
            return Image.LoadAsync(cacheKey, imageUrl, cancellationToken);
        }

        private string? GetCurrentImageUrl()
        {
            return IsFlipped
                ? InteractiveCard.Back?.ImageUrl
                : InteractiveCard.Front.ImageUrl;
        }
    }
}
