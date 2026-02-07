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
        public ICommand GoToTablesPageCommand { get; }
        public ICommand GoToDeckEditorPageCommand {  get; }

        public NavigationBarViewModel(MainWindowViewModel mainVM) 
        {
            _mainVM = mainVM;
            ExecuteLogoutCommand = new RelayCommand(o => LogoutCommand());
            GoToTablesPageCommand = new RelayCommand((o) => GoToTablesPage());
            GoToDeckEditorPageCommand = new RelayCommand((o) => GoToDeckEditorPage());
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

        private void GoToTablesPage()
        {
            if (_mainVM.CurrentView is TablesPageViewModel)
                return;
            _mainVM.CurrentView = new TablesPageViewModel(this._mainVM);
        }

        private void GoToDeckEditorPage()
        {
            if (_mainVM.CurrentView is DeckEditorPageViewModel)
                return;
            _mainVM.CurrentView = new DeckEditorPageViewModel(this._mainVM);
        }
    }
}
