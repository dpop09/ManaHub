using ManaHub.MVVMs;
using System.Windows.Input;

namespace ManaHub.ViewModels
{
    internal class LoginPageViewModel : ViewModelBase
    {
        private MainWindowViewModel _mainVM;
        public ICommand GoToCreateAccountPageCommand { get; }

        // We pass the MainViewModel through the constructor
        public LoginPageViewModel(MainWindowViewModel mainVM)
        {
            _mainVM = mainVM;
            GoToCreateAccountPageCommand = new RelayCommand(o => GoToCreateAccountPage());
        }

        private void GoToCreateAccountPage()
        {
            _mainVM.CurrentView = new CreateAccountPageViewModel(this._mainVM);
        }
    }
}
