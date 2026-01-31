using ManaHub.Models;
using ManaHub.MVVMs;
using ManaHub.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ManaHub.ViewModels
{
    internal class DeckEditorPageViewModel : ViewModelBase 
    {
        private MainWindowViewModel _mainVM;
        public ObservableCollection<Card> FilteredCards {  get; set; }
        public int CardCount => FilteredCards.Count;
        public CardDisplayViewModel CardDisplayVM { get; set; }

        public ICommand GoToLoginPageCommand { get; }

        public DeckEditorPageViewModel(MainWindowViewModel mainVM)
        {
            _mainVM = mainVM;

            FilteredCards = new ObservableCollection<Card>();
            // subscribe to changes in the collection
            FilteredCards.CollectionChanged += (s, e) => OnPropertyChanged(nameof(CardCount));
            CardDisplayVM = new CardDisplayViewModel(this);

            LoadInitialCards();
        }

        private void LoadInitialCards()
        {
            // retrive the cards from the db
            var cards = DatabaseService.Instance.GetCards(limit: 40000);

            // clear and fill the observable collection
            FilteredCards.Clear();
            foreach(var card in cards) 
                FilteredCards.Add(card);
        }
    }
}
