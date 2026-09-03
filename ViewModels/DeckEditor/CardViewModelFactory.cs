using ManaHub.Contracts;
using ManaHub.Domain;

namespace ManaHub.ViewModels
{
    internal sealed class CardViewModelFactory
    {
        private readonly ICardImageService _images;

        public CardViewModelFactory(ICardImageService images)
        {
            _images = images;
        }

        public CardDisplayViewModel CreateDisplay() => new(_images);

        public InteractiveCardViewModel CreateInteractive(Card card) => new(card, _images);
    }
}
