using System.ComponentModel;
using System.IO;
using System.Windows.Input;

using ManaHub.Contracts;
using ManaHub.Domain;
using ManaHub.MVVMs;

namespace ManaHub.ViewModels
{
    internal sealed class DeckDocumentViewModel : ViewModelBase
    {
        private readonly DeckWorkspaceViewModel _workspace;
        private readonly ICardRepository _cards;
        private readonly IDeckService _decks;
        private readonly IFileDialogService _fileDialogs;
        private readonly IDialogService _dialogs;
        private readonly AsyncRelayCommand _saveCommand;
        private readonly AsyncRelayCommand _loadCommand;
        private bool _isBusy;

        public DeckDocumentViewModel(
            DeckWorkspaceViewModel workspace,
            ICardRepository cards,
            IDeckService decks,
            IFileDialogService fileDialogs,
            IDialogService dialogs)
        {
            _workspace = workspace;
            _cards = cards;
            _decks = decks;
            _fileDialogs = fileDialogs;
            _dialogs = dialogs;

            NewCommand = new RelayCommand(_ => NewDeck(), _ => _workspace.HasCards && !IsBusy);
            _saveCommand = new AsyncRelayCommand(
                (_, cancellationToken) => SaveAsync(cancellationToken),
                _ => _workspace.HasCards && !IsBusy);
            _loadCommand = new AsyncRelayCommand(
                (_, cancellationToken) => LoadAsync(cancellationToken),
                _ => !IsBusy);
            SaveCommand = _saveCommand;
            LoadCommand = _loadCommand;

            _workspace.PropertyChanged += OnWorkspacePropertyChanged;
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (_isBusy == value)
                    return;

                _isBusy = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
                _saveCommand.NotifyCanExecuteChanged();
                _loadCommand.NotifyCanExecuteChanged();
            }
        }

        public ICommand NewCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }

        private void NewDeck()
        {
            if (!_workspace.HasCards)
                return;

            if (_dialogs.Confirm("Clear current deck?", "New Deck"))
                _workspace.Clear();
        }

        private Task SaveAsync(CancellationToken cancellationToken)
        {
            return RunAsync(async token =>
            {
                string? path = _fileDialogs.SelectDeckToSave();
                if (path == null)
                    return;

                string name = Path.GetFileNameWithoutExtension(path);
                var document = new DeckDocument(
                    name,
                    _workspace.MainDeckCards.Select(card => card.Id),
                    _workspace.SideboardCards.Select(card => card.Id));

                await _decks.SaveToFileAsync(path, document, token);
                _workspace.Rename(name);
            }, "Deck Save Error", "The deck could not be saved.", cancellationToken);
        }

        private Task LoadAsync(CancellationToken cancellationToken)
        {
            return RunAsync(async token =>
            {
                string? path = _fileDialogs.SelectDeckToOpen();
                if (path == null)
                    return;

                var document = await _decks.LoadFromFileAsync(path, token);
                var uniqueIds = document.MainDeckIds
                    .Concat(document.SideboardIds)
                    .Distinct(StringComparer.Ordinal)
                    .ToArray();
                var loadedCards = await _cards.GetCardsByIdsAsync(uniqueIds, token);
                var cardLibrary = loadedCards.ToDictionary(card => card.Id, StringComparer.Ordinal);

                var mainDeck = ResolveCards(document.MainDeckIds, cardLibrary);
                var sideboard = ResolveCards(document.SideboardIds, cardLibrary);
                _workspace.Replace(document.Name, mainDeck, sideboard);

                int missingCardCount = uniqueIds.Count(id => !cardLibrary.ContainsKey(id));
                if (missingCardCount > 0)
                {
                    _dialogs.ShowMessage(
                        $"{missingCardCount} card(s) in the deck file were not found in the local catalog.",
                        "Deck Loaded With Missing Cards");
                }
            }, "Deck Load Error", "The deck could not be loaded.", cancellationToken);
        }

        private static IReadOnlyList<Card> ResolveCards(
            IEnumerable<string> ids,
            IReadOnlyDictionary<string, Card> cardLibrary)
        {
            return ids
                .Where(cardLibrary.ContainsKey)
                .Select(id => cardLibrary[id])
                .ToArray();
        }

        private async Task RunAsync(
            Func<CancellationToken, Task> operation,
            string errorTitle,
            string errorMessage,
            CancellationToken cancellationToken)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
                await operation(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                _dialogs.ShowMessage($"{errorMessage}\n\n{ex.Message}", errorTitle);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnWorkspacePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DeckWorkspaceViewModel.HasCards))
            {
                CommandManager.InvalidateRequerySuggested();
                _saveCommand.NotifyCanExecuteChanged();
            }
        }
    }
}
