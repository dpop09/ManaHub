using ManaHub.Models;
using System.ComponentModel;

using ManaHub.Domain;

namespace ManaHub.Contracts
{
    internal interface IUserRepository
    {
        Task<bool> CheckUserAsync(string username, string password, CancellationToken cancellationToken = default);
        Task<bool> CheckExistUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<bool> CreateUserAccountAsync(string username, string password, CancellationToken cancellationToken = default);
    }

    internal interface ICardRepository
    {
        Task<long> GetCardCountAsync(CancellationToken cancellationToken = default);
        Task<List<Card>> GetCardsAsync(int limit = 100, CancellationToken cancellationToken = default);
        Task<List<Card>> GetCardsByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
        Task<List<Card>> GetCardsByFilteredSearchAsync(
            string filter,
            bool inName,
            bool inTypes,
            bool inRules,
            CancellationToken cancellationToken = default);
    }

    internal interface ICardCatalogImporter
    {
        Task ImportAsync(string filePath, CancellationToken cancellationToken = default);
    }

    internal interface IDeckService
    {
        Task SaveToFileAsync(
            string path,
            string name,
            IEnumerable<Card> main,
            IEnumerable<Card> side,
            CancellationToken cancellationToken = default);
        Task<DeckSaveModel> LoadFromFileAsync(string path, CancellationToken cancellationToken = default);
    }

    internal interface IDatabaseInitializer
    {
        Task InitializeAsync(CancellationToken cancellationToken = default);
    }

    internal interface IAsyncInitializable
    {
        Task InitializeAsync(CancellationToken cancellationToken = default);
    }

    internal enum AppPage
    {
        Login,
        CreateAccount,
        Tables,
        Game,
        DeckEditor,
        Settings
    }

    internal interface INavigationService
    {
        object? CurrentView { get; }
        event EventHandler? CurrentViewChanged;
        Task NavigateToAsync(AppPage page, CancellationToken cancellationToken = default);
    }

    internal interface ISessionService : INotifyPropertyChanged
    {
        string Username { get; set; }
        void Clear();
    }

    internal interface IDialogService
    {
        void ShowMessage(string message, string title = "Notification");
        bool Confirm(string message, string title);
    }

    internal interface IFileDialogService
    {
        string? SelectDeckToOpen();
        string? SelectDeckToSave();
    }

    internal interface IWindowService
    {
        void Minimize();
        void ToggleMaximize();
        void Close();
    }

    internal interface IApplicationInitializer
    {
        Task InitializeAsync(CancellationToken cancellationToken = default);
    }
}
