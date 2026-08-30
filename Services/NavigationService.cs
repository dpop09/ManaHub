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

        public async Task NavigateToAsync(AppPage page, CancellationToken cancellationToken = default)
        {
            if (!_pageFactories.TryGetValue(page, out var factory))
                throw new InvalidOperationException($"No view model is registered for {page}.");

            object? previousView = CurrentView;
            CurrentView = factory();
            CurrentViewChanged?.Invoke(this, EventArgs.Empty);

            if (previousView is IDisposable disposable)
                disposable.Dispose();

            if (CurrentView is IAsyncInitializable initializable)
                await initializable.InitializeAsync(cancellationToken);
        }
    }
}
