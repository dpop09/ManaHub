using ManaHub.Domain;
using ManaHub.MVVMs;

namespace ManaHub.ViewModels
{
    internal sealed class DeckSelectionViewModel : ViewModelBase, IDisposable
    {
        private Card? _selectedCollectionCard;
        private Card? _selectedMainDeckCard;
        private Card? _selectedSideboardCard;

        public DeckSelectionViewModel(CardDisplayViewModel cardDisplay)
        {
            CardDisplay = cardDisplay;
        }

        public CardDisplayViewModel CardDisplay { get; }

        public Card? SelectedCollectionCard
        {
            get => _selectedCollectionCard;
            set => SetSelection(value, SelectionSource.Collection);
        }

        public Card? SelectedMainDeckCard
        {
            get => _selectedMainDeckCard;
            set => SetSelection(value, SelectionSource.MainDeck);
        }

        public Card? SelectedSideboardCard
        {
            get => _selectedSideboardCard;
            set => SetSelection(value, SelectionSource.Sideboard);
        }

        public void Dispose()
        {
            CardDisplay.Dispose();
        }

        private void SetSelection(Card? card, SelectionSource source)
        {
            ref Card? selected = ref GetSelection(source);
            if (ReferenceEquals(selected, card))
                return;

            selected = card;
            if (card != null)
            {
                _selectedCollectionCard = source == SelectionSource.Collection ? card : null;
                _selectedMainDeckCard = source == SelectionSource.MainDeck ? card : null;
                _selectedSideboardCard = source == SelectionSource.Sideboard ? card : null;
                CardDisplay.Show(card);

                OnPropertyChanged(nameof(SelectedCollectionCard));
                OnPropertyChanged(nameof(SelectedMainDeckCard));
                OnPropertyChanged(nameof(SelectedSideboardCard));
                return;
            }

            OnPropertyChanged(GetPropertyName(source));
        }

        private ref Card? GetSelection(SelectionSource source)
        {
            if (source == SelectionSource.Collection)
                return ref _selectedCollectionCard;
            if (source == SelectionSource.MainDeck)
                return ref _selectedMainDeckCard;
            return ref _selectedSideboardCard;
        }

        private static string GetPropertyName(SelectionSource source)
        {
            return source switch
            {
                SelectionSource.Collection => nameof(SelectedCollectionCard),
                SelectionSource.MainDeck => nameof(SelectedMainDeckCard),
                _ => nameof(SelectedSideboardCard)
            };
        }

        private enum SelectionSource
        {
            Collection,
            MainDeck,
            Sideboard
        }
    }
}
