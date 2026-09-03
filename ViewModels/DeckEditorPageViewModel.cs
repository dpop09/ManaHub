using System.ComponentModel;

using ManaHub.Contracts;
using ManaHub.MVVMs;

namespace ManaHub.ViewModels
{
    internal sealed class DeckEditorPageViewModel : ViewModelBase, IAsyncInitializable, IDisposable
    {
        public DeckEditorPageViewModel(
            CardCatalogViewModel catalog,
            DeckWorkspaceViewModel deck,
            DeckSelectionViewModel selection,
            DeckDocumentViewModel documents)
        {
            Catalog = catalog;
            Deck = deck;
            Selection = selection;
            Documents = documents;

            Catalog.PropertyChanged += OnCollaboratorPropertyChanged;
            Documents.PropertyChanged += OnCollaboratorPropertyChanged;
        }

        public CardCatalogViewModel Catalog { get; }
        public DeckWorkspaceViewModel Deck { get; }
        public DeckSelectionViewModel Selection { get; }
        public DeckDocumentViewModel Documents { get; }
        public bool IsLoading => Catalog.IsBusy || Documents.IsBusy;

        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            return Catalog.InitializeAsync(cancellationToken);
        }

        public void Dispose()
        {
            Catalog.PropertyChanged -= OnCollaboratorPropertyChanged;
            Documents.PropertyChanged -= OnCollaboratorPropertyChanged;
            Documents.Dispose();
            Selection.Dispose();
            Deck.Dispose();
            Catalog.Dispose();
        }

        private void OnCollaboratorPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CardCatalogViewModel.IsBusy)
                || e.PropertyName == nameof(DeckDocumentViewModel.IsBusy))
            {
                OnPropertyChanged(nameof(IsLoading));
            }
        }
    }
}
