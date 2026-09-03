using ManaHub.Contracts;
using ManaHub.Services;
using ManaHub.ViewModels;
using System.IO;
using System.Net.Http;

namespace ManaHub
{
    internal sealed class CompositionRoot : IDisposable
    {
        private readonly NavigationService _navigation;
        private readonly IApplicationInitializer _initializer;
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly HttpClient _imageHttpClient;

        public CompositionRoot()
        {
            // Preserve the existing database location while making its ownership explicit.
            // Moving it to LocalApplicationData should be handled by a separate data migration.
            var connectionFactory = new SqliteConnectionFactory("Data Source=manahub.db");
            IDatabaseInitializer databaseInitializer = new SqliteDatabaseInitializer(connectionFactory);
            IUserRepository users = new SqliteUserRepository(connectionFactory);
            ICardRepository cards = new SqliteCardRepository(connectionFactory);
            ICardCatalogImporter cardCatalogImporter = new CardCatalogImporter(connectionFactory);
            IDeckService decks = new DeckService();
            IDialogService dialogs = new WpfDialogService();
            IFileDialogService fileDialogs = new WpfFileDialogService();
            IWindowService windows = new WpfWindowService();
            ISessionService session = new SessionService();
            _imageHttpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
            _imageHttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ManaHub/1.0");
            string imageCacheDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ManaHub",
                "Cache");
            ICardImageService images = new CardImageService(_imageHttpClient, imageCacheDirectory);
            var cardViewModels = new CardViewModelFactory(images);

            _navigation = new NavigationService();
            _navigation.Register(AppPage.Login,
                () => new LoginPageViewModel(_navigation, users, session, dialogs));
            _navigation.Register(AppPage.CreateAccount,
                () => new CreateAccountPageViewModel(_navigation, users, dialogs));
            _navigation.Register(AppPage.Tables,
                () => new TablesPageViewModel(_navigation));
            _navigation.Register(AppPage.Game,
                () => new GamePageViewModel(_navigation));
            _navigation.Register(AppPage.DeckEditor,
                () =>
                {
                    var catalog = new CardCatalogViewModel(cards, dialogs);
                    var workspace = new DeckWorkspaceViewModel(cardViewModels);
                    var selection = new DeckSelectionViewModel(cardViewModels.CreateDisplay());
                    var documents = new DeckDocumentViewModel(
                        workspace,
                        cards,
                        decks,
                        fileDialogs,
                        dialogs);
                    return new DeckEditorPageViewModel(
                        catalog,
                        workspace,
                        selection,
                        documents);
                });
            _navigation.Register(AppPage.Settings,
                () => new SettingsPageViewModel());

            var navigationBar = new NavigationBarViewModel(_navigation, session);
            _mainWindowViewModel = new MainWindowViewModel(_navigation, windows, navigationBar);

            string cardCatalogPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Data",
                "oracle-cards-20260117221532.json");
            _initializer = new ApplicationInitializer(
                databaseInitializer,
                cards,
                cardCatalogImporter,
                cardCatalogPath);
        }

        public Task InitializeAsync(CancellationToken cancellationToken = default)
            => _initializer.InitializeAsync(cancellationToken);

        public async Task<MainWindow> CreateMainWindowAsync(CancellationToken cancellationToken = default)
        {
            await _navigation.NavigateToAsync(AppPage.Login, cancellationToken);
            return new MainWindow { DataContext = _mainWindowViewModel };
        }

        public void Dispose()
        {
            if (_navigation.CurrentView is IDisposable disposable)
                disposable.Dispose();

            _imageHttpClient.Dispose();
        }
    }
}
