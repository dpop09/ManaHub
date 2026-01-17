using ManaHub.MVVMs;
using ManaHub.Services;
using System.Windows;
using System.Windows.Input;

namespace ManaHub.ViewModels
{
    internal class LoginPageViewModel : ViewModelBase
    {
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
            // hard-coded username and password
            if (DatabaseService.Instance.CheckUser("dpop", "mypassword"))
                GoToDeckEditorPage();
            else
                MessageBox.Show("Incorrect username or password.", "Notification", MessageBoxButton.OK);
        }
    }
}
