using ManaHub.MVVMs;
using System.Windows;

using ManaHub.Contracts;

namespace ManaHub.ViewModels
{
    // The shell observes navigation state and exposes it to the main window.
    internal class MainWindowViewModel : ViewModelBase
    {
        private readonly INavigationService _navigation;
        private readonly IWindowService _windowService;
        private object? _currentView;

        public object? CurrentView
        {
            get => _currentView;
            private set
            { 
                _currentView = value; 
                OnPropertyChanged();
                // whenever the view changes, reevaluate the need to show the nav bar
                UpdateNavVisibility();
            }
        }
        private Visibility _navVisibility = Visibility.Collapsed;
        public Visibility NavVisibility
        {
            get => _navVisibility;
            set
            {
                _navVisibility = value;
                OnPropertyChanged();
            }
        }
        public NavigationBarViewModel NavVM { get; }
        public AsyncRelayCommand ShowGoToCreateAccountPageCommand { get; }
        public RelayCommand CloseWindowCommand { get; }
        public RelayCommand MinimizeWindowCommand { get; }
        public RelayCommand MaximizeWindowCommand { get; }

        public MainWindowViewModel(
            INavigationService navigation,
            IWindowService windowService,
            NavigationBarViewModel navigationBar)
        {
            _navigation = navigation;
            _windowService = windowService;
            NavVM = navigationBar;

            _navigation.CurrentViewChanged += OnCurrentViewChanged;
            
            // Commands to swap the view
            ShowGoToCreateAccountPageCommand = new AsyncRelayCommand(
                (o, cancellationToken) => _navigation.NavigateToAsync(AppPage.CreateAccount, cancellationToken));
            MinimizeWindowCommand = new RelayCommand(o => _windowService.Minimize());
            MaximizeWindowCommand = new RelayCommand(o => _windowService.ToggleMaximize());
            CloseWindowCommand = new RelayCommand(o => _windowService.Close());
        }

        private void OnCurrentViewChanged(object? sender, EventArgs e)
        {
            CurrentView = _navigation.CurrentView;
        }

        private void UpdateNavVisibility()
        {
            // make the nav bar invisibile when user is not logged in
            if (CurrentView is LoginPageViewModel || CurrentView is CreateAccountPageViewModel)
                NavVisibility = Visibility.Collapsed;
            else
                NavVisibility = Visibility.Visible;
        }
    }
}
