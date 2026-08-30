namespace ManaHub.ViewModels
{
    internal sealed class DeckGroupViewModel : IDisposable
    {
        public DeckGroupViewModel(int manaValue, IReadOnlyList<InteractiveCardViewModel> cards)
        {
            ManaValue = manaValue;
            Cards = cards;
        }

        public int ManaValue { get; }
        public IReadOnlyList<InteractiveCardViewModel> Cards { get; }

        public void Dispose()
        {
            foreach (var card in Cards)
                card.Dispose();
        }
    }
}
