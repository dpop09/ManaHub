using Microsoft.Data.Sqlite;

namespace ManaHub.Services
{
    internal sealed class SqliteConnectionFactory
    {
        private readonly string _connectionString;

        public SqliteConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqliteConnection CreateConnection() => new(_connectionString);
    }
}
