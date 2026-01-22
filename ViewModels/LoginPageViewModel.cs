using ManaHub.MVVMs;
using ManaHub.Services;
using System.Windows;
using System.Windows.Input;

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

        private MainWindowViewModel _mainVM;
        public ICommand GoToCreateAccountPageCommand { get; }
        public ICommand ExecuteLoginCommand { get; }

        // We pass the MainViewModel through the constructor
        public LoginPageViewModel(MainWindowViewModel mainVM)
        {
            _mainVM = mainVM;
            var db = DatabaseService.Instance;
            GoToCreateAccountPageCommand = new RelayCommand(o => GoToCreateAccountPage());
            ExecuteLoginCommand = new RelayCommand(o => ExecuteLogin());
        }

        private void GoToCreateAccountPage()
        {
            _mainVM.CurrentView = new CreateAccountPageViewModel(this._mainVM);
        }
        private void GoToDeckEditorPage()
        {
            _mainVM.CurrentView = new DeckEditorPageViewModel(this._mainVM);
        }
        private void ExecuteLogin()
        {
            // ensure username and password is not null or whitespace
            if (string.IsNullOrWhiteSpace(_username) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Please fill in all fields.", "Notification", MessageBoxButton.OK);
                return;
            }
            // check database if user exists, else display incorrect message
            if (DatabaseService.Instance.CheckUser(Username, Password))
            {
                _mainVM.NavVM.Username = Username;
                GoToDeckEditorPage();
            }
            else
                MessageBox.Show("Incorrect username or password.", "Notification", MessageBoxButton.OK);
        }
    }
}
