using ManaHub.MVVMs;
using System.Windows.Input;

using ManaHub.Contracts;

namespace ManaHub.ViewModels
{
    internal class LoginPageViewModel : ViewModelBase
    {
        private string _username = string.Empty;
        private string _password = string.Empty;
        public string Username 
        {
            get { return  _username; }
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }
        public string Password 
        { 
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private readonly INavigationService _navigation;
        private readonly IUserRepository _users;
        private readonly ISessionService _session;
        private readonly IDialogService _dialogs;
        public ICommand GoToCreateAccountPageCommand { get; }
        public ICommand ExecuteLoginCommand { get; }

        public LoginPageViewModel(
            INavigationService navigation,
            IUserRepository users,
            ISessionService session,
            IDialogService dialogs)
        {
            _navigation = navigation;
            _users = users;
            _session = session;
            _dialogs = dialogs;
            GoToCreateAccountPageCommand = new AsyncRelayCommand(
                (o, cancellationToken) => GoToCreateAccountPageAsync(cancellationToken));
            ExecuteLoginCommand = new AsyncRelayCommand(
                (o, cancellationToken) => ExecuteLoginAsync(cancellationToken));
        }

        private Task GoToCreateAccountPageAsync(CancellationToken cancellationToken)
        {
            return _navigation.NavigateToAsync(AppPage.CreateAccount, cancellationToken);
        }

        private Task GoToTablesPageAsync(CancellationToken cancellationToken)
        {
            return _navigation.NavigateToAsync(AppPage.Tables, cancellationToken);
        }

        private async Task ExecuteLoginAsync(CancellationToken cancellationToken)
        {
            // ensure username and password is not null or whitespace
            if (string.IsNullOrWhiteSpace(_username) || string.IsNullOrWhiteSpace(Password))
            {
                _dialogs.ShowMessage("Please fill in all fields.");
                return;
            }
            // check database if user exists, else display incorrect message
            if (await _users.CheckUserAsync(Username, Password, cancellationToken))
            {
                _session.Username = Username;
                await GoToTablesPageAsync(cancellationToken);
            }
            else
                _dialogs.ShowMessage("Incorrect username or password.");
        }
    }
}
