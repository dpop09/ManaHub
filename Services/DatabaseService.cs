using Microsoft.Data.Sqlite;

using ManaHub.Contracts;

namespace ManaHub.Services
{
    internal sealed partial class DatabaseService : IUserRepository, ICardRepository, IDatabaseInitializer
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            // create the file and table if they don't exist
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                var command = connection.CreateCommand();
                command.CommandText = @"
                       CREATE TABLE IF NOT EXISTS Users (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Username TEXT NOT NULL UNIQUE,
                            Password Text NOT NULL
                       );";
                command.CommandText += @"
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
}
