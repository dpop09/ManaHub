using ManaHub.MVVMs;
using System.Windows.Input;

using ManaHub.Contracts;

namespace ManaHub.ViewModels
{
    internal class LoginPageViewModel : ViewModelBase
    {
        private string _username;
        private string _password;
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
            GoToCreateAccountPageCommand = new RelayCommand(o => GoToCreateAccountPage());
            ExecuteLoginCommand = new RelayCommand(o => ExecuteLogin());
        }

        private void GoToCreateAccountPage()
        {
            _navigation.NavigateTo(AppPage.CreateAccount);
        }
        private void GoToTablesPage()
        {
            _navigation.NavigateTo(AppPage.Tables);
        }
        private void ExecuteLogin()
        {
            // ensure username and password is not null or whitespace
            if (string.IsNullOrWhiteSpace(_username) || string.IsNullOrWhiteSpace(Password))
            {
                _dialogs.ShowMessage("Please fill in all fields.");
                return;
            }
            // check database if user exists, else display incorrect message
            if (_users.CheckUser(Username, Password))
            {
                _session.Username = Username;
                GoToTablesPage();
            }
            else
                _dialogs.ShowMessage("Incorrect username or password.");
        }
    }
}
