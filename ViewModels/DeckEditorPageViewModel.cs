using ManaHub.Models;
using ManaHub.MVVMs;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

using ManaHub.Contracts;

namespace ManaHub.ViewModels
{
    internal class DeckEditorPageViewModel : ViewModelBase
    {
        private readonly ICardRepository _cards;
        private readonly IDeckService _decks;
        private readonly IFileDialogService _fileDialogs;
        private readonly IDialogService _dialogs;
        private Card _selectedCollectionCard;
        private Card _selectedMainDeckCard;
        private Card _selectedSideboardCard;
        private string _deckName = "";
        private string _filterSearchText = "";
        private bool _searchNames = true; // Default to true
        private bool _searchTypes = false;
        private bool _searchRules = false;
        public ObservableCollection<Card> FilteredCards { get; set; }
        public ObservableCollection<Card> DeckList { get; set; }
        public ObservableCollection<Card> SideboardList { get; set; }
        public CardDisplayViewModel CardDisplayVM { get; set; }
        public ObservableCollection<DeckGroup> GroupedDeckColumns { get; set; }
        public Card SelectedCollectionCard
        {
            get => _selectedCollectionCard;
            set
            {
                if (_selectedCollectionCard == value) 
                    return;
                _selectedCollectionCard = value;
                if (value != null)
                {
                    _selectedMainDeckCard = null;
                    _selectedSideboardCard = null;
                    CardDisplayVM.CardDisplay = value;
                    // Notify all three so the other grids clear their highlights
                    OnPropertyChanged(nameof(SelectedCollectionCard));
                    OnPropertyChanged(nameof(SelectedMainDeckCard));
                    OnPropertyChanged(nameof(SelectedSideboardCard));
                }
                OnPropertyChanged();
            }
        }
        public Card SelectedMainDeckCard 
        {
            get => _selectedMainDeckCard;
            set
            {
                if (_selectedMainDeckCard == value)
                    return;
                _selectedMainDeckCard = value;
                if (value != null)
                {
                    _selectedCollectionCard = null;
                    _selectedSideboardCard = null;
                    CardDisplayVM.CardDisplay = value;
                    OnPropertyChanged(nameof(SelectedCollectionCard));
                    OnPropertyChanged(nameof(SelectedMainDeckCard));
                    OnPropertyChanged(nameof(SelectedSideboardCard));
                }
                OnPropertyChanged();
            }
        }
        public Card SelectedSideboardCard 
        {
            get => _selectedSideboardCard;
            set
            {
                if (_selectedSideboardCard == value)
                    return;
                _selectedSideboardCard = value;
                if (value != null)
                {
                    _selectedCollectionCard = null;
                    _selectedMainDeckCard = null;
                    CardDisplayVM.CardDisplay = value;
                    OnPropertyChanged(nameof(SelectedCollectionCard));
                    OnPropertyChanged(nameof(SelectedMainDeckCard));
                    OnPropertyChanged(nameof(SelectedSideboardCard));
                }
                OnPropertyChanged();
            }
        }
        public string DeckName 
        {
            get => _deckName;
            set
            {
                _deckName = value;
                OnPropertyChanged();
            }
        }
        public string FilterSearchText 
        {
            get => _filterSearchText;
            set
            {
                _filterSearchText = value;
                OnPropertyChanged();
            }
        }
        public bool SearchNames 
        { 
            get => _searchNames; 
            set 
            { 
                _searchNames = value; 
                OnPropertyChanged();
            } 
        }
        public bool SearchTypes 
        { 
            get => _searchTypes; 
            set 
            { 
                _searchTypes = value;
                OnPropertyChanged(); 
            } 
        }
        public bool SearchRules 
        { 
            get => _searchRules; 
            set 
            { 
                _searchRules = value; 
                OnPropertyChanged(); 
            } 
        }
        public int CardCount => FilteredCards.Count;
        public int MainDeckCardCount => DeckList.Count;
        public int MainDeckLandCount => GetMainDeckLandCount();
        public int MainDeckCreatureCount => GetMainDeckCreatureCount();
        public int SideboardCardCount => SideboardList.Count;
        public ICommand AddToDeckCommand { get; }
        public ICommand RemoveFromDeckCommand { get; }
        public ICommand AddToSideboardCommand { get; }
        public ICommand RemoveFromSideboardCommand { get; }
        public ICommand MoveCardFromMainDeckToSideboardCommand {  get; }
        public ICommand MoveCardFromSideboardToMainDeckCommand { get; }
        public ICommand NewDeckCommand { get; }
        public ICommand SaveDeckCommand { get; }
        public ICommand LoadDeckCommand { get; }
        public ICommand FilterSearchCardsCommand { get; }
        public ICommand ClearFilterSearchCommand { get; }

        public DeckEditorPageViewModel(
            ICardRepository cards,
            IDeckService decks,
            IFileDialogService fileDialogs,
            IDialogService dialogs)
        {
            _cards = cards;
            _decks = decks;
            _fileDialogs = fileDialogs;
            _dialogs = dialogs;

            // initialize collections
            FilteredCards = new ObservableCollection<Card>();
            DeckList = new ObservableCollection<Card>();
            SideboardList = new ObservableCollection<Card>();

            GroupedDeckColumns = new ObservableCollection<DeckGroup>();

            // subscribe to changes in the collection
            FilteredCards.CollectionChanged += (s, e) => OnPropertyChanged(nameof(CardCount));
            DeckList.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(MainDeckCardCount));
                OnPropertyChanged(nameof(MainDeckLandCount));
                OnPropertyChanged(nameof(MainDeckCreatureCount));
                UpdateGroupedDeck();
            };
            SideboardList.CollectionChanged += (s, e) => OnPropertyChanged(nameof(SideboardCardCount));

            // commands
            AddToDeckCommand = new RelayCommand((obj) => AddToDeck(obj));
            RemoveFromDeckCommand = new RelayCommand((obj) => RemoveFromDeck(obj));
            AddToSideboardCommand = new RelayCommand((obj) => AddToSideboard(obj));
            RemoveFromSideboardCommand = new RelayCommand((obj) => RemoveFromSideboard(obj));
            MoveCardFromMainDeckToSideboardCommand = new RelayCommand((obj) => MoveCardFromMainDeckToSideboard(obj));
            MoveCardFromSideboardToMainDeckCommand = new RelayCommand((obj) => MoveCardFromSideboardToMainDeck(obj));
            NewDeckCommand = new RelayCommand(o => NewDeck());
            SaveDeckCommand = new RelayCommand(o => SaveDeck());
            LoadDeckCommand = new RelayCommand(o => LoadDeck());
            FilterSearchCardsCommand = new RelayCommand(o => FilterSearchCards());
            ClearFilterSearchCommand = new RelayCommand(o => ClearFilterSearch());

            CardDisplayVM = new CardDisplayViewModel(this);

            LoadInitialCards();
        }

        private void LoadInitialCards()
        {
            // retrive the cards from the db
            var cards = _cards.GetCards(limit: 40000);

            // clear and fill the observable collection
            FilteredCards.Clear();
            foreach (var card in cards)
                FilteredCards.Add(card);
        }

        private void AddToDeck(object obj)
        {
            if (obj == null)
                return;
            else if (obj is Card card)
                DeckList.Add(card);
        }
        private void RemoveFromDeck(object obj)
        {
            if (obj == null)
                return;
            else if (obj is Card card)
                DeckList.Remove(card);
        }
        private void AddToSideboard(object obj)
        {
            if (obj == null)
                return;
            else if (obj is Card card)
                SideboardList.Add(card);
        }
        private void RemoveFromSideboard(object obj)
        {
            if (obj == null)
                return;
            else if (obj is Card card)
                SideboardList.Remove(card);
        }
        private void MoveCardFromMainDeckToSideboard(object obj)
        {
            AddToSideboard(obj);
            RemoveFromDeck(obj);
        }
        private void MoveCardFromSideboardToMainDeck(object obj)
        {
            AddToDeck(obj);
            RemoveFromSideboard(obj);
        }
        private void NewDeck()
        {
            if (DeckList.Count == 0 && SideboardList.Count == 0)
                return;

            if (!_dialogs.Confirm("Clear current deck?", "New Deck"))
                return;

            DeckList.Clear();
            SideboardList.Clear();
            DeckName = "";
        }
        private void SaveDeck()
        {
            // check if there are any cards
            if (!DeckList.Any() && !SideboardList.Any())
                return;
            string? path = _fileDialogs.SelectDeckToSave();
            if (path == null)
                return;

            DeckName = Path.GetFileNameWithoutExtension(path);
            _decks.SaveToFile(path, DeckName, DeckList, SideboardList);
        }
        private void LoadDeck()
        {
            string? path = _fileDialogs.SelectDeckToOpen();
            if (path != null)
            {
                var data = _decks.LoadFromFile(path);

                DeckName = data.DeckName;
                // 1. Fetch unique card data from DB (one instance per ID)
                var uniqueIds = data.MainDeckIds.Distinct().Concat(data.SideboardIds.Distinct());
                // Quick lookup table
                var cardLibrary = _cards.GetCardsByIds(uniqueIds).ToDictionary(c => c.Id);

                DeckList.Clear();
                // 2. Loop through the ORIGINAL ID list (which contains duplicates)
                foreach (var id in data.MainDeckIds)
                {
                    if (cardLibrary.TryGetValue(id, out var card))
                    {
                        // We add the same card object reference multiple times
                        DeckList.Add(card);
                    }
                }

                SideboardList.Clear();
                foreach (var id in data.SideboardIds)
                {
                    if (cardLibrary.TryGetValue(id, out var card))
                    {
                        SideboardList.Add(card);
                    }
                }
            }
        }
        private void FilterSearchCards()
        {
            if (string.IsNullOrWhiteSpace(FilterSearchText))
                return;
            var cards = _cards.GetCardsByFilteredSearch(FilterSearchText, SearchNames, SearchTypes, SearchRules);
            // clear and fill the observable collection
            FilteredCards.Clear();
            foreach (var card in cards)
                FilteredCards.Add(card);
        }
        private void ClearFilterSearch()
        {
            FilterSearchText = "";
            LoadInitialCards();
        }
        private int GetMainDeckLandCount() 
            => DeckList.Count(c => c.TypeLine != null && c.TypeLine.Contains("Land"));
        private int GetMainDeckCreatureCount()
            => DeckList.Count(c => c.TypeLine != null && c.TypeLine.Contains("Creature"));

        private void UpdateGroupedDeck()
        {
            var groups = DeckList
                .GroupBy(c => (int)c.Cmc)
                .OrderBy(g => g.Key)
                .Select(g => new DeckGroup
                {
                    ManaValue = g.Key,
                    Cards = new ObservableCollection<Card>(g.ToList())
                })
                .ToList();
            // Clear and refill to keep the same collection reference for the UI
            GroupedDeckColumns.Clear();
            foreach (var group in groups)
            {
                GroupedDeckColumns.Add(group);
            }
        }
    }
}
