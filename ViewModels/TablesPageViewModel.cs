using ManaHub.MVVMs;
using System.Windows.Input;

using ManaHub.Contracts;

namespace ManaHub.ViewModels
{
    internal class TablesPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigation;

        public ICommand GoToGamePageCommand { get; }

        public TablesPageViewModel(INavigationService navigation)
        {
            _navigation = navigation;
            GoToGamePageCommand = new RelayCommand((o) => GoToGamePage());
        }

        private void GoToGamePage()
        {
            _navigation.NavigateTo(AppPage.Game);
        }
    }
}
