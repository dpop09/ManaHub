using ManaHub.Models;
using ManaHub.MVVMs;

namespace ManaHub.ViewModels
{
    internal class InteractiveCardViewModel : ViewModelBase
    {
        private readonly ViewModelBase _vm;
        private Card _interactiveCard;
        public Card InteractiveCard 
        { 
            get; 
            set; 
        }

        public InteractiveCardViewModel(ViewModelBase vm) 
        {
            _vm = vm;
        }
    }
}
