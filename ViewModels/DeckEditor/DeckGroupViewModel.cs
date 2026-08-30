namespace ManaHub.ViewModels
{
    internal sealed class DeckGroupViewModel
    {
        public DeckGroupViewModel(int manaValue, IReadOnlyList<InteractiveCardViewModel> cards)
        {
            ManaValue = manaValue;
            Cards = cards;
        }

        public int ManaValue { get; }
        public IReadOnlyList<InteractiveCardViewModel> Cards { get; }
    }
}
