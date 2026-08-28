using ManaHub.Contracts;
using ManaHub.Services;
using ManaHub.ViewModels;
using System.IO;

namespace ManaHub
{
    internal sealed class CompositionRoot
    {
        private readonly NavigationService _navigation;
        private readonly IApplicationInitializer _initializer;
        private readonly MainWindowViewModel _mainWindowViewModel;

        public CompositionRoot()
        {
            // Preserve the existing database location while making its ownership explicit.
            // Moving it to LocalApplicationData should be handled by a separate data migration.
            var database = new DatabaseService("Data Source=manahub.db");

            IUserRepository users = database;
            ICardRepository cards = database;
            IDeckService decks = new DeckService();
            IDialogService dialogs = new WpfDialogService();
            IFileDialogService fileDialogs = new WpfFileDialogService();
            IWindowService windows = new WpfWindowService();
            ISessionService session = new SessionService();

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
                () => new DeckEditorPageViewModel(cards, decks, fileDialogs, dialogs));
            _navigation.Register(AppPage.Settings,
                () => new SettingsPageViewModel());

            var navigationBar = new NavigationBarViewModel(_navigation, session);
            _mainWindowViewModel = new MainWindowViewModel(_navigation, windows, navigationBar);

            string cardCatalogPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Data",
                "oracle-cards-20260117221532.json");
            _initializer = new ApplicationInitializer(cards, cardCatalogPath);
        }

        public Task InitializeAsync() => _initializer.InitializeAsync();

        public MainWindow CreateMainWindow()
        {
            _navigation.NavigateTo(AppPage.Login);
            return new MainWindow { DataContext = _mainWindowViewModel };
        }
    }
}
