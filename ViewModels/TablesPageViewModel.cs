using ManaHub.MVVMs;
using System.Windows.Input;

namespace ManaHub.ViewModels
{
    internal class TablesPageViewModel : ViewModelBase
    {
        private MainWindowViewModel _mainVM;

        public ICommand GoToGamePageCommand { get; }

        public TablesPageViewModel(MainWindowViewModel mainVM)
        {
            _mainVM = mainVM;
            GoToGamePageCommand = new RelayCommand((o) => GoToGamePage());
        }

        private void GoToGamePage()
        {
            _mainVM.CurrentView = new GamePageViewModel(this._mainVM);
        }
    }
}
