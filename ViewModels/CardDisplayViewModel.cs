using ManaHub.Models;
using ManaHub.MVVMs;

namespace ManaHub.ViewModels
{
    class CardDisplayViewModel : ViewModelBase
    {
        private Card _cardDisplay;
        public Card CardDisplay 
        {
            get => _cardDisplay;
            set 
            { 
                _cardDisplay = value; 
                OnPropertyChanged();
            }
        }

        private readonly DeckEditorPageViewModel _depvm;

        public CardDisplayViewModel(DeckEditorPageViewModel depvm)
        {
            _depvm = depvm;
        }
    }
}
