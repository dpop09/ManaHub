using ManaHub.MVVMs;
using ManaHub.Services;
using System.Windows;
using System.Windows.Input;

namespace ManaHub.ViewModels
{
    internal class CreateAccountPageViewModel : ViewModelBase
    {
        private MainWindowViewModel _mainVM;
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

        public CreateAccountPageViewModel(MainWindowViewModel mainVM)
        {
            _mainVM = mainVM;
            GoToLoginPageCommand = new RelayCommand(o => GoToLoginPage());
            ExecuteCreateAccountCommand = new RelayCommand(o =>  ExecuteCreateAccount());
        }

        private void GoToLoginPage()
        {
            _mainVM.CurrentView = new LoginPageViewModel(this._mainVM);
        }

        private void ExecuteCreateAccount()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Please fill in all fields.", "Notification", MessageBoxButton.OK);
                return;
            }
            if (DatabaseService.Instance.CheckExistUsername(Username))
            {
                string message = $"\"{Username}\" already exists. Please choose a different username.";
                MessageBox.Show(message, "Notification", MessageBoxButton.OK);
                return;
            }
            if (DatabaseService.Instance.CreateUserAccount(Username, Password))
            {
                MessageBox.Show("Your account has been created successfully.", "Notification", MessageBoxButton.OK);
                GoToLoginPage();
            }
            else
            {
                MessageBox.Show("Something has gone wrong with the database. " +
                    "Your account cannot be created at this time.", "Notification", MessageBoxButton.OK);
                return;
            }
        }
    }
}
