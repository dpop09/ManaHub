using ManaHub.Models;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Text.Json;

namespace ManaHub.Services
{
    sealed partial class DatabaseService
    {
        public async Task BulkImportCards(string filePath)
        {
            using var stream = File.OpenRead(filePath);

            var cards = JsonSerializer.DeserializeAsyncEnumerable<Card>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.Transaction = transaction;
                    command.CommandText = @"
                        INSERT INTO Cards (Name, ManaCost, TypeLine, OracleText) 
                        VALUES ($name, $mana, $type, $text)";

                    var pName = command.Parameters.Add("$name", SqliteType.Text);
                    var pMana = command.Parameters.Add("$mana", SqliteType.Text);
                    var pType = command.Parameters.Add("$type", SqliteType.Text);
                    var pText = command.Parameters.Add("$text", SqliteType.Text);

                    await foreach (var card in cards)
                    {
                        if (card == null) continue;

                        pName.Value = card.Name ?? (object)DBNull.Value;
                        pMana.Value = card.ManaCost ?? (object)DBNull.Value;
                        pType.Value = card.TypeLine ?? (object)DBNull.Value;
                        pText.Value = card.OracleText ?? (object)DBNull.Value;

                        await command.ExecuteNonQueryAsync();
                    }
                    await transaction.CommitAsync();
                }
            }
        }

        public long GetCardCount()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM Cards";
                return (long)command.ExecuteScalar();
            }
        }

        public List<Card> GetCards(int limit = 100)
        {
            List<Card> cardList = new List<Card>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT Name, ManaCost, TypeLine, OracleText FROM Cards LIMIT $limit";
                command.Parameters.AddWithValue("$limit", limit);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cardList.Add(new Card
                        {
                            Name = reader.IsDBNull(0) ? "" : reader.GetString(0),
                            ManaCost = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            TypeLine = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            OracleText = reader.IsDBNull(3) ? "" : reader.GetString(3)
                        });
                    }
                }
            }
            return cardList;
        }
    }
}
