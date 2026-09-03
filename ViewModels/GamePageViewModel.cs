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
            GoToTablesPageCommand = new AsyncRelayCommand(
                (o, cancellationToken) => GoToTablesPageAsync(cancellationToken));
        }

        private Task GoToTablesPageAsync(CancellationToken cancellationToken)
        {
            return _navigation.NavigateToAsync(AppPage.Tables, cancellationToken);
        }
    }
}
