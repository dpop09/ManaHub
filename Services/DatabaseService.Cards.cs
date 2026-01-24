using ManaHub.Models;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Text.Json;

namespace ManaHub.Services
{
    internal sealed partial class DatabaseService
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
                        INSERT INTO Cards (Name, Colors, ManaCost, Cmc, TypeLine, [Set], Power, Toughness, Rarity, CollectorNumber, OracleText) 
                        VALUES ($name, $colors, $mana, $cmc, $type, $set, $power, $tough, $rarity, $colnum, $text)";

                    var pName = command.Parameters.Add("$name", SqliteType.Text);
                    var pColors = command.Parameters.Add("$colors", SqliteType.Text);
                    var pMana = command.Parameters.Add("$mana", SqliteType.Text);
                    var pCmc = command.Parameters.Add("$cmc", SqliteType.Real);
                    var pType = command.Parameters.Add("$type", SqliteType.Text);
                    var pSet = command.Parameters.Add("$set", SqliteType.Text);
                    var pPower = command.Parameters.Add("$power", SqliteType.Text);
                    var pTough = command.Parameters.Add("$tough", SqliteType.Text);
                    var pRarity = command.Parameters.Add("$rarity", SqliteType.Text);
                    var pColNum = command.Parameters.Add("$colnum", SqliteType.Text);
                    var pText = command.Parameters.Add("$text", SqliteType.Text);

                    await foreach (var card in cards)
                    {
                        if (card == null) continue;

                        pName.Value = card.Name ?? (object)DBNull.Value;
                        if (card.Colors != null && card.Colors.Any())
                            pColors.Value = string.Join(",", card.Colors);
                        else
                            pColors.Value = DBNull.Value; // colorless
                        pMana.Value = card.ManaCost ?? (object)DBNull.Value;
                        pCmc.Value = card.Cmc;
                        pType.Value = card.TypeLine ?? (object)DBNull.Value;
                        // convert set text from json to uppercase if not null
                        pSet.Value = card.Set?.ToUpper() ?? (object)DBNull.Value;
                        pPower.Value = card.Power ?? (object)DBNull.Value;
                        pTough.Value = card.Toughness ?? (object)DBNull.Value;
                        pRarity.Value = !string.IsNullOrEmpty(card.Rarity)
                            ? char.ToUpper(card.Rarity[0]) + card.Rarity.Substring(1).ToLower()
                            : (object)DBNull.Value;
                        pColNum.Value = card.CollectorNumber ?? (object)DBNull.Value;
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
                command.CommandText = "SELECT Name, Colors, ManaCost, Cmc, TypeLine, [Set], Power, " +
                    "Toughness, Rarity, CollectorNumber, OracleText FROM Cards LIMIT $limit";
                command.Parameters.AddWithValue("$limit", limit);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cardList.Add(new Card
                        {
                            Name = reader.IsDBNull(0) ? "" : reader.GetString(0),
                            ColorsString = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            ManaCost = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            Cmc = reader.IsDBNull(3) ? 0.0 : reader.GetDouble(3),
                            TypeLine = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            Set = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            Power = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            Toughness = reader.IsDBNull(7) ? "" : reader.GetString(7),
                            Rarity = reader.IsDBNull(8) ? "" : reader.GetString(8),
                            CollectorNumber = reader.IsDBNull(9) ? "" : reader.GetString(9),
                            OracleText = reader.IsDBNull(10) ? "" : reader.GetString(10)
                        });
                    }
                }
            }
            return cardList;
        }
    }
}
