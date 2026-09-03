using ManaHub.MVVMs;
using ManaHub.Services;
using System.Windows;
using System.Windows.Input;

using ManaHub.Contracts;

namespace ManaHub.ViewModels
{
    internal class CreateAccountPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigation;
        private readonly IUserRepository _users;
        private readonly IDialogService _dialogs;
        private string _username = string.Empty;
        private string _password = string.Empty;
        public string Username
        {
            get { return _username; }
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
        public ICommand GoToLoginPageCommand { get; }
        public ICommand ExecuteCreateAccountCommand { get; }

        public CreateAccountPageViewModel(
            INavigationService navigation,
            IUserRepository users,
            IDialogService dialogs)
        {
            _navigation = navigation;
            _users = users;
            _dialogs = dialogs;
            GoToLoginPageCommand = new AsyncRelayCommand(
                (o, cancellationToken) => GoToLoginPageAsync(cancellationToken));
            ExecuteCreateAccountCommand = new AsyncRelayCommand(
                (o, cancellationToken) => ExecuteCreateAccountAsync(cancellationToken));
        }

        private Task GoToLoginPageAsync(CancellationToken cancellationToken)
        {
            return _navigation.NavigateToAsync(AppPage.Login, cancellationToken);
        }

        private async Task ExecuteCreateAccountAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                _dialogs.ShowMessage("Please fill in all fields.");
                return;
            }
            if (await _users.CheckExistUsernameAsync(Username, cancellationToken))
            {
                string message = $"\"{Username}\" already exists. Please choose a different username.";
                _dialogs.ShowMessage(message);
                return;
            }
            if (await _users.CreateUserAccountAsync(Username, Password, cancellationToken))
            {
                _dialogs.ShowMessage("Your account has been created successfully.");
                await GoToLoginPageAsync(cancellationToken);
            }
            else
            {
                _dialogs.ShowMessage("Something has gone wrong with the database. " +
                    "Your account cannot be created at this time.");
                return;
            }
        }
    }
}
