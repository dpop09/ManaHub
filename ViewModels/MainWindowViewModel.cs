using ManaHub.MVVMs;

namespace ManaHub.ViewModels
{
    // MainWindowViewModel acts as the "mediator" for views to communicate indirectly when page navigation occurs
    internal class MainWindowViewModel : ViewModelBase
    {
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); } // Uses your OnPropertyChanged
        }
        public RelayCommand ShowGoToCreateAccountPageCommand {  get; set; }

        public MainWindowViewModel()
        {
            // Initial Page
            CurrentView = new LoginPageViewModel(this);

            // Commands to swap the view
            ShowGoToCreateAccountPageCommand = new RelayCommand(o => CurrentView = new CreateAccountPageViewModel(this));
        }
    }
}
