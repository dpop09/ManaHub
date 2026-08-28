using ManaHub.Models;
using System.ComponentModel;

namespace ManaHub.Contracts
{
    internal interface IUserRepository
    {
        bool CheckUser(string username, string password);
        bool CheckExistUsername(string username);
        bool CreateUserAccount(string username, string password);
    }

    internal interface ICardRepository
    {
        Task BulkImportCards(string filePath);
        long GetCardCount();
        List<Card> GetCards(int limit = 100);
        List<Card> GetCardsByIds(IEnumerable<string> ids);
        List<Card> GetCardsByFilteredSearch(string filter, bool inName, bool inTypes, bool inRules);
    }

    internal interface IDeckService
    {
        void SaveToFile(string path, string name, IEnumerable<Card> main, IEnumerable<Card> side);
        DeckSaveModel LoadFromFile(string path);
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
        void NavigateTo(AppPage page);
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
        Task InitializeAsync();
    }
}
