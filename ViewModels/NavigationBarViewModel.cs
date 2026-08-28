using ManaHub.MVVMs;
using System.Windows.Input;

using ManaHub.Contracts;
using System.ComponentModel;

namespace ManaHub.ViewModels
{
    internal class NavigationBarViewModel : ViewModelBase
    {
        private readonly INavigationService _navigation;
        private readonly ISessionService _session;

        public string Username => _session.Username;
        public ICommand ExecuteLogoutCommand { get; }
        public ICommand GoToTablesPageCommand { get; }
        public ICommand GoToDeckEditorPageCommand { get; }
        public ICommand GoToSettingsPageCommand { get; }

        public NavigationBarViewModel(INavigationService navigation, ISessionService session)
        {
            _navigation = navigation;
            _session = session;
            _session.PropertyChanged += OnSessionPropertyChanged;
            ExecuteLogoutCommand = new RelayCommand(o => LogoutCommand());
            GoToTablesPageCommand = new RelayCommand((o) => GoToTablesPage());
            GoToDeckEditorPageCommand = new RelayCommand((o) => GoToDeckEditorPage());
            GoToSettingsPageCommand = new RelayCommand((o) => GoToSettingsPage());
        }

        private void LogoutCommand()
        {
            _session.Clear();
            _navigation.NavigateTo(AppPage.Login);
        }

        private void GoToTablesPage()
        {
            if (_navigation.CurrentView is TablesPageViewModel)
                return;
            _navigation.NavigateTo(AppPage.Tables);
        }

        private void GoToSettingsPage()
        {
            if (_navigation.CurrentView is SettingsPageViewModel)
                return;
            _navigation.NavigateTo(AppPage.Settings);
        }

        private void GoToDeckEditorPage()
        {
            if (_navigation.CurrentView is DeckEditorPageViewModel)
                return;
            _navigation.NavigateTo(AppPage.DeckEditor);
        }

        private void OnSessionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ISessionService.Username))
                OnPropertyChanged(nameof(Username));
        }
    }
}
