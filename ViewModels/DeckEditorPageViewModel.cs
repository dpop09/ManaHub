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
        public ICommand GoToLoginPageCommand { get; }

        public DeckEditorPageViewModel(MainWindowViewModel mainVM)
        {
            _mainVM = mainVM;
            FilteredCards = new ObservableCollection<Card>();
            LoadInitialCards();
            GoToLoginPageCommand = new RelayCommand(o => GoToLoginPage());
        }

        private void GoToLoginPage()
        {
            _mainVM.CurrentView = new LoginPageViewModel(this._mainVM);
        }

        private void LoadInitialCards()
        {
            var cards = DatabaseService.Instance.GetCards(limit: 100);
            foreach(var card in cards) FilteredCards.Add(card);
        }
    }
}
