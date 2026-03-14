using ManaHub.Models;
using ManaHub.MVVMs;
using System.Windows.Input;

namespace ManaHub.ViewModels
{
    class CardDisplayViewModel : ViewModelBase
    {
        private readonly DeckEditorPageViewModel _depvm;
        private Card _cardDisplay;
        public Card CardDisplay 
        {
            get => _cardDisplay;
            set 
            { 
                _cardDisplay = value;
                IsFlipped = false;
                OnPropertyChanged();
            }
        }
        private bool _isFlipped;
        public bool IsFlipped 
        {
            get => _isFlipped;
            set
            {
                _isFlipped = value;
                OnPropertyChanged();
            } 
        }
        public ICommand FlipCardCommand { get; }
        

        public CardDisplayViewModel(DeckEditorPageViewModel depvm)
        {
            _depvm = depvm;
            FlipCardCommand = new RelayCommand(o => IsFlipped = !IsFlipped);
        }
    }
}
