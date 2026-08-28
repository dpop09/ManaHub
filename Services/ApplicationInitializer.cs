using ManaHub.Contracts;
using System.IO;

namespace ManaHub.Services
{
    internal sealed class ApplicationInitializer : IApplicationInitializer
    {
        private readonly ICardRepository _cards;
        private readonly string _cardCatalogPath;

        public ApplicationInitializer(ICardRepository cards, string cardCatalogPath)
        {
            _cards = cards;
            _cardCatalogPath = cardCatalogPath;
        }

        public async Task InitializeAsync()
        {
            if (_cards.GetCardCount() != 0 || !File.Exists(_cardCatalogPath))
                return;

            await _cards.BulkImportCards(_cardCatalogPath);
        }
    }
}
