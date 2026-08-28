using ManaHub.Contracts;
using System.IO;

namespace ManaHub.Services
{
    internal sealed class ApplicationInitializer : IApplicationInitializer
    {
        private readonly ICardRepository _cards;
        private readonly ICardCatalogImporter _cardCatalogImporter;
        private readonly IDatabaseInitializer _database;
        private readonly string _cardCatalogPath;

        public ApplicationInitializer(
            IDatabaseInitializer database,
            ICardRepository cards,
            ICardCatalogImporter cardCatalogImporter,
            string cardCatalogPath)
        {
            _database = database;
            _cards = cards;
            _cardCatalogImporter = cardCatalogImporter;
            _cardCatalogPath = cardCatalogPath;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            await _database.InitializeAsync(cancellationToken);

            if (await _cards.GetCardCountAsync(cancellationToken) != 0)
                return;

            if (!File.Exists(_cardCatalogPath))
                throw new FileNotFoundException("The card catalog required for first-run setup was not found.", _cardCatalogPath);

            await _cardCatalogImporter.ImportAsync(_cardCatalogPath, cancellationToken);
        }
    }
}
