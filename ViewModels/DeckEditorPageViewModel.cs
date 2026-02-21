using ManaHub.Models;
using ManaHub.MVVMs;
using ManaHub.Services;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace ManaHub.ViewModels
{
    internal class DeckEditorPageViewModel : ViewModelBase
    {
        private MainWindowViewModel _mainVM;
        private Card _selectedCollectionCard;
        private Card _selectedMainDeckCard;
        private Card _selectedSideboardCard;
        private string _deckName = "";
        public ObservableCollection<Card> FilteredCards { get; set; }
        public ObservableCollection<Card> DeckList { get; set; }
        public ObservableCollection<Card> SideboardList { get; set; }
        public CardDisplayViewModel CardDisplayVM { get; set; }
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

        public int CardCount => FilteredCards.Count;
        public int MainDeckCardCount => DeckList.Count;
        public int SideboardCardCount => SideboardList.Count;

        public ICommand GoToLoginPageCommand { get; }
        public ICommand AddToDeckCommand { get; }
        public ICommand RemoveFromDeckCommand { get; }
        public ICommand AddToSideboardCommand { get; }
        public ICommand RemoveFromSideboardCommand { get; }
        public ICommand MoveCardFromMainDeckToSideboardCommand {  get; }
        public ICommand MoveCardFromSideboardToMainDeckCommand { get; }
        public ICommand NewDeckCommand { get; }
        public ICommand SaveDeckCommand { get; }
        public ICommand LoadDeckCommand { get; }

        public DeckEditorPageViewModel(MainWindowViewModel mainVM)
        {
            _mainVM = mainVM;

            // initialize collections
            FilteredCards = new ObservableCollection<Card>();
            DeckList = new ObservableCollection<Card>();
            SideboardList = new ObservableCollection<Card>();

            // subscribe to changes in the collection
            FilteredCards.CollectionChanged += (s, e) => OnPropertyChanged(nameof(CardCount));
            DeckList.CollectionChanged += (s, e) => OnPropertyChanged(nameof(MainDeckCardCount));
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

            CardDisplayVM = new CardDisplayViewModel(this);

            LoadInitialCards();
        }

        private void LoadInitialCards()
        {
            // retrive the cards from the db
            var cards = DatabaseService.Instance.GetCards(limit: 40000);

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
            if (DeckList.Count > 0 && MessageBox.Show("Clear current deck?", "New Deck", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DeckList.Clear();
                SideboardList.Clear();
                DeckName = "";
            }
        }
        private void SaveDeck()
        {
            // check if there are any cards
            if (!DeckList.Any() && !SideboardList.Any())
                return;
            var sfd = new Microsoft.Win32.SaveFileDialog { Filter = "ManaHub Deck (*.json)|*.json" };
            if (sfd.ShowDialog() == true)
                DeckName = Path.GetFileNameWithoutExtension(sfd.SafeFileName);
                DeckService.SaveToFile(sfd.FileName, DeckName, DeckList, SideboardList);
        }
        private void LoadDeck()
        {
            var ofd = new Microsoft.Win32.OpenFileDialog { Filter = "ManaHub Deck (*.json)|*.json" };
            if (ofd.ShowDialog() == true)
            {
                var data = DeckService.LoadFromFile(ofd.FileName);

                DeckName = data.DeckName;
                // 1. Fetch unique card data from DB (one instance per ID)
                var uniqueIds = data.MainDeckIds.Distinct().Concat(data.SideboardIds.Distinct());
                // Quick lookup table
                var cardLibrary = DatabaseService.Instance.GetCardsByIds(uniqueIds).ToDictionary(c => c.Id); 

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
    }
}
