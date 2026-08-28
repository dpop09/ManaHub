using ManaHub.Contracts;
using ManaHub.Models;

namespace ManaHub.Services
{
    internal sealed class SqliteCardRepository : ICardRepository
    {
        private readonly SqliteConnectionFactory _connections;

        public SqliteCardRepository(SqliteConnectionFactory connections)
        {
            _connections = connections;
        }

        public async Task<long> GetCardCountAsync(CancellationToken cancellationToken = default)
        {
            using var connection = _connections.CreateConnection();
            await connection.OpenAsync(cancellationToken);

            var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Cards";
            return (long)(await command.ExecuteScalarAsync(cancellationToken) ?? 0L);
        }

        public async Task<List<Card>> GetCardsAsync(
            int limit = 100,
            CancellationToken cancellationToken = default)
        {
            using var connection = _connections.CreateConnection();
            await connection.OpenAsync(cancellationToken);

            var command = connection.CreateCommand();
            command.CommandText = $@"
                SELECT {CardRecordMapper.SelectColumns}
                FROM Cards
                LIMIT $limit";
            command.Parameters.AddWithValue("$limit", limit);

            return await ReadCardsAsync(command, cancellationToken);
        }

        public async Task<List<Card>> GetCardsByIdsAsync(
            IEnumerable<string> ids,
            CancellationToken cancellationToken = default)
        {
            var idList = ids.Distinct().ToList();
            if (idList.Count == 0)
                return new List<Card>();

            using var connection = _connections.CreateConnection();
            await connection.OpenAsync(cancellationToken);

            var command = connection.CreateCommand();
            var parameterNames = idList.Select((id, index) => $"$id{index}").ToArray();
            command.CommandText = $@"
                SELECT {CardRecordMapper.SelectColumns}
                FROM Cards
                WHERE Id IN ({string.Join(",", parameterNames)})";

            for (int index = 0; index < parameterNames.Length; index++)
                command.Parameters.AddWithValue(parameterNames[index], idList[index]);

            return await ReadCardsAsync(command, cancellationToken);
        }

        public async Task<List<Card>> GetCardsByFilteredSearchAsync(
            string filter,
            bool inName,
            bool inTypes,
            bool inRules,
            CancellationToken cancellationToken = default)
        {
            if (!inName && !inTypes && !inRules)
                inName = true;

            var filters = new List<string>();
            if (inName)
            {
                filters.Add("Name LIKE $filter");
                filters.Add("SecondName LIKE $filter");
            }
            if (inTypes)
            {
                filters.Add("TypeLine LIKE $filter");
                filters.Add("SecondTypeLine LIKE $filter");
            }
            if (inRules)
            {
                filters.Add("OracleText LIKE $filter");
                filters.Add("SecondOracleText LIKE $filter");
            }

            using var connection = _connections.CreateConnection();
            await connection.OpenAsync(cancellationToken);

            var command = connection.CreateCommand();
            command.CommandText = $@"
                SELECT {CardRecordMapper.SelectColumns}
                FROM Cards
                WHERE ({string.Join(" OR ", filters)})";
            command.Parameters.AddWithValue("$filter", $"%{filter}%");

            return await ReadCardsAsync(command, cancellationToken);
        }

        private static async Task<List<Card>> ReadCardsAsync(
            Microsoft.Data.Sqlite.SqliteCommand command,
            CancellationToken cancellationToken)
        {
            var cards = new List<Card>();
            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
                cards.Add(CardRecordMapper.Map(reader));

            return cards;
        }
    }
}
