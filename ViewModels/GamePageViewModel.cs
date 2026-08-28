using ManaHub.MVVMs;
using System.Windows.Input;

using ManaHub.Contracts;

namespace ManaHub.ViewModels
{
    internal class GamePageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigation;
        public ICommand GoToTablesPageCommand { get; }


        public GamePageViewModel(INavigationService navigation)
        {
            _navigation = navigation;
            GoToTablesPageCommand = new RelayCommand((o) => GoToTablesPage());
        }

        private void GoToTablesPage()
        {
            _navigation.NavigateTo(AppPage.Tables);
        }
    }
}
