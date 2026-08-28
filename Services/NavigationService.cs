using ManaHub.Contracts;

namespace ManaHub.Services
{
    internal sealed class NavigationService : INavigationService
    {
        private readonly Dictionary<AppPage, Func<object>> _pageFactories = new();

        public object? CurrentView { get; private set; }
        public event EventHandler? CurrentViewChanged;

        public void Register(AppPage page, Func<object> factory)
        {
            _pageFactories[page] = factory;
        }

        public void NavigateTo(AppPage page)
        {
            if (!_pageFactories.TryGetValue(page, out var factory))
                throw new InvalidOperationException($"No view model is registered for {page}.");

            CurrentView = factory();
            CurrentViewChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
