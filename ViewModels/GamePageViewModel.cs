using ManaHub.MVVMs;
using System.Windows.Input;

namespace ManaHub.ViewModels
{
    internal class GamePageViewModel : ViewModelBase
    {
        private MainWindowViewModel _mainVM;
        public ICommand GoToTablesPageCommand { get; }


        public GamePageViewModel(MainWindowViewModel mainVM)
        {
            _mainVM = mainVM;
            GoToTablesPageCommand = new RelayCommand((o) => GoToTablesPage());
        }

        private void GoToTablesPage()
        {
            _mainVM.CurrentView = new TablesPageViewModel(this._mainVM);
        }
    }
}
