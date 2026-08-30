using System.Windows.Input;

using ManaHub.Contracts;
using ManaHub.Domain;
using ManaHub.MVVMs;

namespace ManaHub.ViewModels
{
    internal sealed class CardDisplayViewModel : ViewModelBase, IDisposable
    {
        private Card? _card;
        private bool _isFlipped;
        private bool _isActive;

        public CardDisplayViewModel(ICardImageService images)
        {
            Image = new AsyncImageViewModel(images);
            FlipCardCommand = new RelayCommand(_ => Flip());
        }

        public Card? CardDisplay
        {
            get => _card;
            private set => _card = value;
        }

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

        public AsyncImageViewModel Image { get; }
        public ICommand FlipCardCommand { get; }

        public void Show(Card card)
        {
            if (ReferenceEquals(CardDisplay, card))
                return;

            CardDisplay = card;
            IsFlipped = false;
            OnPropertyChanged(nameof(CardDisplay));

            if (_isActive)
                StartCurrentImageLoad();
            else
                Image.Clear();
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
            if (!TryGetImageRequest(out string cacheKey, out string imageUrl))
            {
                Image.Clear();
                return;
            }

            Image.StartLoad(cacheKey, imageUrl);
        }

        private Task LoadCurrentImageAsync(CancellationToken cancellationToken)
        {
            if (!TryGetImageRequest(out string cacheKey, out string imageUrl))
            {
                Image.Clear();
                return Task.CompletedTask;
            }

            return Image.LoadAsync(cacheKey, imageUrl, cancellationToken);
        }

        private bool TryGetImageRequest(out string cacheKey, out string imageUrl)
        {
            cacheKey = string.Empty;
            imageUrl = string.Empty;
            if (CardDisplay == null)
                return false;

            string? selectedImageUrl = IsFlipped
                ? CardDisplay.Back?.ImageUrl
                : CardDisplay.Front.ImageUrl;
            if (string.IsNullOrWhiteSpace(selectedImageUrl))
                return false;

            cacheKey = $"{CardDisplay.Id}_{(IsFlipped ? "back" : "front")}";
            imageUrl = selectedImageUrl;
            return true;
        }
    }
}
