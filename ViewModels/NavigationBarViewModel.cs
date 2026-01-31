using ManaHub.MVVMs;
using System.Windows.Input;

namespace ManaHub.ViewModels
{
    internal class NavigationBarViewModel : ViewModelBase
    {
        private string _username;
        public string Username 
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }
        private MainWindowViewModel _mainVM;
        public ICommand ExecuteLogoutCommand { get; }

        public NavigationBarViewModel(MainWindowViewModel mainVM) 
        {
            _mainVM = mainVM;
            ExecuteLogoutCommand = new RelayCommand(o => LogoutCommand());
        }

        private void LogoutCommand()
        {
            if (_mainVM == null) 
                return;
            // clear session
            Username = string.Empty;
            // navigate to the login page
            _mainVM.CurrentView = new LoginPageViewModel(this._mainVM);
        }
    }
}
