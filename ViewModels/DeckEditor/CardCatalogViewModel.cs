using System.Collections.ObjectModel;
using System.Windows.Input;

using ManaHub.Contracts;
using ManaHub.Domain;
using ManaHub.MVVMs;

namespace ManaHub.ViewModels
{
    internal sealed class CardCatalogViewModel : ViewModelBase, IAsyncInitializable
    {
        private const int InitialCardLimit = 40000;

        private readonly ICardRepository _cards;
        private readonly IDialogService _dialogs;
        private string _searchText = string.Empty;
        private bool _searchNames = true;
        private bool _searchTypes;
        private bool _searchRules;
        private bool _isBusy;
        private readonly AsyncRelayCommand _searchCommand;
        private readonly AsyncRelayCommand _clearSearchCommand;

        public CardCatalogViewModel(ICardRepository cards, IDialogService dialogs)
        {
            _cards = cards;
            _dialogs = dialogs;

            Cards.CollectionChanged += (_, _) => OnPropertyChanged(nameof(CardCount));
            _searchCommand = new AsyncRelayCommand(
                (_, cancellationToken) => SearchAsync(cancellationToken),
                _ => !IsBusy);
            _clearSearchCommand = new AsyncRelayCommand(
                (_, cancellationToken) => ClearSearchAsync(cancellationToken),
                _ => !IsBusy);
            SearchCommand = _searchCommand;
            ClearSearchCommand = _clearSearchCommand;
        }

        public ObservableCollection<Card> Cards { get; } = new();

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText == value)
                    return;

                _searchText = value;
                OnPropertyChanged();
            }
        }

        public bool SearchNames
        {
            get => _searchNames;
            set
            {
                if (_searchNames == value)
                    return;

                _searchNames = value;
                OnPropertyChanged();
            }
        }

        public bool SearchTypes
        {
            get => _searchTypes;
            set
            {
                if (_searchTypes == value)
                    return;

                _searchTypes = value;
                OnPropertyChanged();
            }
        }

        public bool SearchRules
        {
            get => _searchRules;
            set
            {
                if (_searchRules == value)
                    return;

                _searchRules = value;
                OnPropertyChanged();
            }
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
                _searchCommand.NotifyCanExecuteChanged();
                _clearSearchCommand.NotifyCanExecuteChanged();
            }
        }

        public int CardCount => Cards.Count;
        public ICommand SearchCommand { get; }
        public ICommand ClearSearchCommand { get; }

        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            return RunAsync(
                LoadInitialCardsAsync,
                "Card Load Error",
                "Cards could not be loaded.",
                cancellationToken,
                propagateCancellation: true);
        }

        private Task SearchAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return Task.CompletedTask;

            return RunAsync(
                async token =>
                {
                    var cards = await _cards.GetCardsByFilteredSearchAsync(
                        SearchText,
                        SearchNames,
                        SearchTypes,
                        SearchRules,
                        token);
                    ReplaceCards(cards);
                },
                "Card Search Error",
                "The card search could not be completed.",
                cancellationToken);
        }

        private Task ClearSearchAsync(CancellationToken cancellationToken)
        {
            SearchText = string.Empty;
            return RunAsync(
                LoadInitialCardsAsync,
                "Card Load Error",
                "Cards could not be reloaded.",
                cancellationToken);
        }

        private async Task LoadInitialCardsAsync(CancellationToken cancellationToken)
        {
            var cards = await _cards.GetCardsAsync(InitialCardLimit, cancellationToken);
            ReplaceCards(cards);
        }

        private void ReplaceCards(IEnumerable<Card> cards)
        {
            Cards.Clear();
            foreach (var card in cards)
                Cards.Add(card);
        }

        private async Task RunAsync(
            Func<CancellationToken, Task> operation,
            string errorTitle,
            string errorMessage,
            CancellationToken cancellationToken,
            bool propagateCancellation = false)
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
                if (propagateCancellation)
                    throw;
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
    }
}
