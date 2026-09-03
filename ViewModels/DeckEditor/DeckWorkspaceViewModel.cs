using System.Collections.ObjectModel;
using System.Windows.Input;

using ManaHub.Domain;
using ManaHub.MVVMs;

namespace ManaHub.ViewModels
{
    internal sealed class DeckWorkspaceViewModel : ViewModelBase, IDisposable
    {
        private readonly CardViewModelFactory _cardViewModels;
        private string _name = string.Empty;
        private bool _isReplacingCards;

        public DeckWorkspaceViewModel(CardViewModelFactory cardViewModels)
        {
            _cardViewModels = cardViewModels;

            MainDeckCards.CollectionChanged += (_, _) => OnMainDeckChanged();
            SideboardCards.CollectionChanged += (_, _) => OnSideboardChanged();

            AddToMainDeckCommand = new RelayCommand(AddToMainDeck);
            RemoveFromMainDeckCommand = new RelayCommand(RemoveFromMainDeck);
            AddToSideboardCommand = new RelayCommand(AddToSideboard);
            RemoveFromSideboardCommand = new RelayCommand(RemoveFromSideboard);
            MoveToSideboardCommand = new RelayCommand(MoveToSideboard);
            MoveToMainDeckCommand = new RelayCommand(MoveToMainDeck);
        }

        public string Name
        {
            get => _name;
            private set
            {
                if (_name == value)
                    return;

                _name = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Card> MainDeckCards { get; } = new();
        public ObservableCollection<Card> SideboardCards { get; } = new();
        public ObservableCollection<DeckGroupViewModel> MainDeckGroups { get; } = new();

        public int MainDeckCardCount => MainDeckCards.Count;
        public int MainDeckLandCount => CountMainDeckCardsOfType("Land");
        public int MainDeckCreatureCount => CountMainDeckCardsOfType("Creature");
        public int SideboardCardCount => SideboardCards.Count;
        public bool HasCards => MainDeckCards.Count > 0 || SideboardCards.Count > 0;

        public ICommand AddToMainDeckCommand { get; }
        public ICommand RemoveFromMainDeckCommand { get; }
        public ICommand AddToSideboardCommand { get; }
        public ICommand RemoveFromSideboardCommand { get; }
        public ICommand MoveToSideboardCommand { get; }
        public ICommand MoveToMainDeckCommand { get; }

        public void Clear()
        {
            Replace(string.Empty, Array.Empty<Card>(), Array.Empty<Card>());
        }

        public void Replace(string name, IEnumerable<Card> mainDeck, IEnumerable<Card> sideboard)
        {
            _isReplacingCards = true;
            try
            {
                MainDeckCards.Clear();
                foreach (var card in mainDeck)
                    MainDeckCards.Add(card);

                SideboardCards.Clear();
                foreach (var card in sideboard)
                    SideboardCards.Add(card);

                Name = name;
            }
            finally
            {
                _isReplacingCards = false;
            }

            NotifyDeckChanged();
            NotifySideboardChanged();
        }

        public void Rename(string name)
        {
            Name = name;
        }

        public void Dispose()
        {
            foreach (var group in MainDeckGroups)
                group.Dispose();

            MainDeckGroups.Clear();
        }

        private void AddToMainDeck(object? value)
        {
            if (value is Card card)
                MainDeckCards.Add(card);
        }

        private void RemoveFromMainDeck(object? value)
        {
            if (value is Card card)
                MainDeckCards.Remove(card);
        }

        private void AddToSideboard(object? value)
        {
            if (value is Card card)
                SideboardCards.Add(card);
        }

        private void RemoveFromSideboard(object? value)
        {
            if (value is Card card)
                SideboardCards.Remove(card);
        }

        private void MoveToSideboard(object? value)
        {
            if (value is not Card card || !MainDeckCards.Remove(card))
                return;

            SideboardCards.Add(card);
        }

        private void MoveToMainDeck(object? value)
        {
            if (value is not Card card || !SideboardCards.Remove(card))
                return;

            MainDeckCards.Add(card);
        }

        private void OnMainDeckChanged()
        {
            if (!_isReplacingCards)
                NotifyDeckChanged();
        }

        private void OnSideboardChanged()
        {
            if (!_isReplacingCards)
                NotifySideboardChanged();
        }

        private void NotifyDeckChanged()
        {
            OnPropertyChanged(nameof(MainDeckCardCount));
            OnPropertyChanged(nameof(MainDeckLandCount));
            OnPropertyChanged(nameof(MainDeckCreatureCount));
            OnPropertyChanged(nameof(HasCards));
            RefreshMainDeckGroups();
        }

        private void NotifySideboardChanged()
        {
            OnPropertyChanged(nameof(SideboardCardCount));
            OnPropertyChanged(nameof(HasCards));
        }

        private int CountMainDeckCardsOfType(string cardType)
        {
            return MainDeckCards.Count(card =>
                card.Front.TypeLine.Contains(cardType, StringComparison.OrdinalIgnoreCase));
        }

        private void RefreshMainDeckGroups()
        {
            var groups = MainDeckCards
                .GroupBy(card => (int)card.ManaValue)
                .OrderBy(group => group.Key)
                .Select(group => new DeckGroupViewModel(
                    group.Key,
                    group.Select(_cardViewModels.CreateInteractive).ToArray()))
                .ToArray();

            foreach (var existingGroup in MainDeckGroups)
                existingGroup.Dispose();

            MainDeckGroups.Clear();
            foreach (var group in groups)
                MainDeckGroups.Add(group);
        }
    }
}
