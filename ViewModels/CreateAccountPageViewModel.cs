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
        private string _username;
        private string _password;
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
            GoToLoginPageCommand = new RelayCommand(o => GoToLoginPage());
            ExecuteCreateAccountCommand = new RelayCommand(o =>  ExecuteCreateAccount());
        }

        private void GoToLoginPage()
        {
            _navigation.NavigateTo(AppPage.Login);
        }

        private void ExecuteCreateAccount()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                _dialogs.ShowMessage("Please fill in all fields.");
                return;
            }
            if (_users.CheckExistUsername(Username))
            {
                string message = $"\"{Username}\" already exists. Please choose a different username.";
                _dialogs.ShowMessage(message);
                return;
            }
            if (_users.CreateUserAccount(Username, Password))
            {
                _dialogs.ShowMessage("Your account has been created successfully.");
                GoToLoginPage();
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
