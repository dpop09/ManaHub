using Microsoft.Data.Sqlite;

namespace ManaHub.Services
{
    // DatabaseService follows singleton design pattern to ensure there is only at least 1 instance of it
    internal sealed partial class DatabaseService
    {
        // static instance that holds our single object
        private static DatabaseService _instance;
        // lock object to make it thread-safe
        private static readonly object _lock = new object();
        public string _connectionString = "Data Source=manahub.db";

        private DatabaseService()
        {
            InitalizeDatabase();
        }

        public static DatabaseService Instance
        {
            get
            {
                // to prevent race conditions
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new DatabaseService();
                    return _instance;
                }
            }
        }

        private void InitalizeDatabase()
        {
            // create the file and table if they don't exist
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                       CREATE TABLE IF NOT EXISTS Users (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Username TEXT NOT NULL UNIQUE,
                            Password Text NOT NULL
                       );";
                command.CommandText += @"
                       CREATE TABLE IF NOT EXISTS Cards (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
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
                            Layout TEXT
                       );";
                command.ExecuteNonQuery();
            }
        }
    }
}
