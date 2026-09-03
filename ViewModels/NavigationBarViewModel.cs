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
            ExecuteLogoutCommand = new AsyncRelayCommand((o, cancellationToken) => LogoutAsync(cancellationToken));
            GoToTablesPageCommand = new AsyncRelayCommand((o, cancellationToken) => GoToTablesPageAsync(cancellationToken));
            GoToDeckEditorPageCommand = new AsyncRelayCommand((o, cancellationToken) => GoToDeckEditorPageAsync(cancellationToken));
            GoToSettingsPageCommand = new AsyncRelayCommand((o, cancellationToken) => GoToSettingsPageAsync(cancellationToken));
        }

        private async Task LogoutAsync(CancellationToken cancellationToken)
        {
            _session.Clear();
            await _navigation.NavigateToAsync(AppPage.Login, cancellationToken);
        }

        private async Task GoToTablesPageAsync(CancellationToken cancellationToken)
        {
            if (_navigation.CurrentView is TablesPageViewModel)
                return;
            await _navigation.NavigateToAsync(AppPage.Tables, cancellationToken);
        }

        private async Task GoToSettingsPageAsync(CancellationToken cancellationToken)
        {
            if (_navigation.CurrentView is SettingsPageViewModel)
                return;
            await _navigation.NavigateToAsync(AppPage.Settings, cancellationToken);
        }

        private async Task GoToDeckEditorPageAsync(CancellationToken cancellationToken)
        {
            if (_navigation.CurrentView is DeckEditorPageViewModel)
                return;
            await _navigation.NavigateToAsync(AppPage.DeckEditor, cancellationToken);
        }

        private void OnSessionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ISessionService.Username))
                OnPropertyChanged(nameof(Username));
        }
    }
}
