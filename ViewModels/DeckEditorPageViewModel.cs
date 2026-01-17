using ManaHub.MVVMs;
using System.Windows.Input;

namespace ManaHub.ViewModels
{
    internal class DeckEditorPageViewModel : ViewModelBase 
    {
        private MainWindowViewModel _mainVM;
        public ICommand GoToLoginPageCommand { get; }

        public DeckEditorPageViewModel(MainWindowViewModel mainVM)
        {
            _mainVM = mainVM;
            GoToLoginPageCommand = new RelayCommand(o => GoToLoginPage());
        }

        private void GoToLoginPage()
        {
            _mainVM.CurrentView = new LoginPageViewModel(this._mainVM);
        }
    }
}
