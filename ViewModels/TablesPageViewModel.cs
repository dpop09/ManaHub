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
            GoToGamePageCommand = new AsyncRelayCommand(
                (o, cancellationToken) => GoToGamePageAsync(cancellationToken));
        }

        private Task GoToGamePageAsync(CancellationToken cancellationToken)
        {
            return _navigation.NavigateToAsync(AppPage.Game, cancellationToken);
        }
    }
}
