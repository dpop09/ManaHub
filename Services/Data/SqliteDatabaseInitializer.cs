using ManaHub.Contracts;

namespace ManaHub.Services
{
    internal sealed class SqliteDatabaseInitializer : IDatabaseInitializer
    {
        private readonly SqliteConnectionFactory _connections;

        public SqliteDatabaseInitializer(SqliteConnectionFactory connections)
        {
            _connections = connections;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            using var connection = _connections.CreateConnection();
            await connection.OpenAsync(cancellationToken);

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Cards (
                    Id TEXT PRIMARY KEY,
                    Name TEXT,
                    Colors TEXT,
                    ManaCost TEXT,
                    Cmc DECIMAL,
                    TypeLine TEXT,
                    [Set] TEXT,
                    Power TEXT,
                    Toughness TEXT,
                    Rarity TEXT,
                    CollectorNumber TEXT,
                    OracleText TEXT,
                    Layout TEXT,
                    ColorIdentity TEXT,
                    PrimaryImageUrl TEXT,
                    SecondName TEXT,
                    SecondManaCost TEXT,
                    SecondTypeLine TEXT,
                    SecondOracleText TEXT,
                    SecondColors TEXT,
                    SecondPower TEXT,
                    SecondToughness TEXT,
                    SecondaryImageUrl TEXT
                );";

            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}
