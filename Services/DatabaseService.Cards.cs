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
                        INSERT INTO Cards (Id, Name, Colors, ManaCost, Cmc, TypeLine, [Set], Power, Toughness, Rarity, CollectorNumber, OracleText, Layout, " +
                        "ColorIdentity, SecondName, SecondManaCost, SecondTypeLine, SecondOracleText, SecondColors, SecondPower, SecondToughness) " +
                        "VALUES ($id, $name, $colors, $mana, $cmc, $type, $set, $power, $tough, $rarity, $colnum, $text, $layout, " +
                        "$coloridentity, $secondname, $secondmana, $secondtype, $secondtext, $secondcolors, $secondpower, $secondtough)";

                    var pId = command.Parameters.Add("$id", SqliteType.Text);
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
                    var pLayout = command.Parameters.Add("$layout", SqliteType.Text);
                    var pColorIdentity = command.Parameters.Add("$coloridentity", SqliteType.Text);
                    var pSecondName = command.Parameters.Add("$secondname", SqliteType.Text);
                    var pSecondMana = command.Parameters.Add("$secondmana", SqliteType.Text);
                    var pSecondType = command.Parameters.Add("$secondtype", SqliteType.Text);
                    var pSecondText = command.Parameters.Add("$secondtext", SqliteType.Text);
                    var pSecondColors = command.Parameters.Add("$secondcolors", SqliteType.Text);
                    var pSecondPower = command.Parameters.Add("$secondpower", SqliteType.Text);
                    var pSecondTough = command.Parameters.Add("$secondtough", SqliteType.Text);

                    await foreach (var card in cards)
                    {
                        if (card == null) 
                            continue;

                        var forbiddenLayouts = new[] { "token", "double_faced_token", "emblem", "art_series", "planar" };
                        var forbiddenSets = new[] { "FJMP", "CMB2", "UNH", "HHO", "JTLA", "OARC", "SUNF", "FCLU", "OPCA", "FLTR", "MOC", "PVAN" };
                        if (forbiddenLayouts.Contains(card.Layout?.ToLower()) || 
                            forbiddenSets.Contains(card.Set?.ToUpper()))
                            continue;
                        

                        // Every card has these attributes
                        pId.Value = card.Id ?? (object)DBNull.Value;
                        pCmc.Value = card.Cmc;
                        pSet.Value = card.Set?.ToUpper() ?? (object)DBNull.Value;
                        pRarity.Value = !string.IsNullOrEmpty(card.Rarity)
                            ? char.ToUpper(card.Rarity[0]) + card.Rarity.Substring(1).ToLower()
                            : (object)DBNull.Value;
                        pColNum.Value = card.CollectorNumber ?? (object)DBNull.Value;
                        pLayout.Value = card.Layout ?? (object)DBNull.Value;
                        if (card.ColorIdentity != null && card.ColorIdentity.Any())
                            pColorIdentity.Value = string.Join(",", card.ColorIdentity);
                        else
                            pColorIdentity.Value = DBNull.Value; // colorless

                        // If the card has multiple faces
                        if (card.CardFaces != null && card.CardFaces.Count >= 2)
                        {
                            var face1 = card.CardFaces[0];
                            var face2 = card.CardFaces[1];

                            pName.Value = face1.Name ?? (object)DBNull.Value;
                            pMana.Value = face1.ManaCost ?? (object)DBNull.Value;
                            pType.Value = face1.TypeLine ?? (object)DBNull.Value;
                            pText.Value = face1.OracleText ?? (object)DBNull.Value;
                            pPower.Value = face1.Power ?? (object)DBNull.Value;
                            pTough.Value = face1.Toughness ?? (object)DBNull.Value;
                            if (face1.Colors != null && face1.Colors.Any())
                                pColors.Value = string.Join(",", face1.Colors);
                            else
                                pColors.Value = DBNull.Value; // colorless

                            pSecondName.Value = face2.Name ?? (object)DBNull.Value;
                            pSecondMana.Value = face2.ManaCost ?? (object)DBNull.Value;
                            pSecondType.Value = face2.TypeLine ?? (object)DBNull.Value;
                            pSecondText.Value = face2.OracleText ?? (object)DBNull.Value;
                            pSecondPower.Value = face2.Power ?? (object)DBNull.Value;
                            pSecondTough.Value = face2.Toughness ?? (object)DBNull.Value;
                            if (face2.Colors != null && face2.Colors.Any())
                                pSecondColors.Value = string.Join(",", face2.Colors);
                            else
                                pSecondColors.Value = DBNull.Value; // colorless
                        }
                        else // If the card does not have multiple faces
                        {
                            pName.Value = card.Name ?? (object)DBNull.Value;
                            pMana.Value = card.ManaCost ?? (object)DBNull.Value;
                            pType.Value = card.TypeLine ?? (object)DBNull.Value;
                            pPower.Value = card.Power ?? (object)DBNull.Value;
                            pTough.Value = card.Toughness ?? (object)DBNull.Value;
                            pText.Value = card.OracleText ?? (object)DBNull.Value;
                            if (card.Colors != null && card.Colors.Any())
                                pColors.Value = string.Join(",", card.Colors);
                            else
                                pColors.Value = DBNull.Value; // colorless

                            pSecondName.Value = DBNull.Value;
                            pSecondMana.Value = DBNull.Value;
                            pSecondType.Value = DBNull.Value;
                            pSecondText.Value = DBNull.Value;
                            pSecondPower.Value = DBNull.Value;
                            pSecondTough.Value = DBNull.Value;
                            pSecondColors.Value = DBNull.Value;
                        }

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
                command.CommandText = "SELECT COUNT(*) FROM Cards ";
                return (long)command.ExecuteScalar();
            }
        }
        private Card MapReaderToCard(SqliteDataReader reader)
        {
            return new Card
            {
                Id = reader.IsDBNull(0) ? "" : reader.GetString(0),
                Name = reader.IsDBNull(1) ? "" : reader.GetString(1),
                ColorsString = reader.IsDBNull(2) ? "" : reader.GetString(2),
                ManaCost = reader.IsDBNull(3) ? "" : reader.GetString(3),
                Cmc = reader.IsDBNull(4) ? 0.0 : reader.GetDouble(4),
                TypeLine = reader.IsDBNull(5) ? "" : reader.GetString(5),
                Set = reader.IsDBNull(6) ? "" : reader.GetString(6),
                Power = reader.IsDBNull(7) ? "" : reader.GetString(7),
                Toughness = reader.IsDBNull(8) ? "" : reader.GetString(8),
                Rarity = reader.IsDBNull(9) ? "" : reader.GetString(9),
                CollectorNumber = reader.IsDBNull(10) ? "" : reader.GetString(10),
                OracleText = reader.IsDBNull(11) ? "" : reader.GetString(11),
                Layout = reader.IsDBNull(12) ? "" : reader.GetString(12),
                ColorIdentityString = reader.IsDBNull(13) ? "" : reader.GetString(13),
                SecondName = reader.IsDBNull(14) ? "" : reader.GetString(14),
                SecondManaCost = reader.IsDBNull(15) ? "" : reader.GetString(15),
                SecondTypeLine = reader.IsDBNull(16) ? "" : reader.GetString(16),
                SecondOracleText = reader.IsDBNull(17) ? "" : reader.GetString(17),
                SecondColorsString = reader.IsDBNull(18) ? "" : reader.GetString(18),
                SecondPower = reader.IsDBNull(19) ? "" : reader.GetString(19),
                SecondToughness = reader.IsDBNull(20) ? "" : reader.GetString(20)
            };
        }
        public List<Card> GetCards(int limit = 100)
        {
            List<Card> cardList = new List<Card>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT 
                        Id, Name, Colors, ManaCost, Cmc, TypeLine, [Set], Power, 
                        Toughness, Rarity, CollectorNumber, OracleText, Layout, ColorIdentity, 
                        SecondName, SecondManaCost, SecondTypeLine, SecondOracleText, 
                        SecondColors, SecondPower, SecondToughness
                    FROM Cards 
                    LIMIT $limit";

                command.Parameters.AddWithValue("$limit", limit);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        cardList.Add(MapReaderToCard(reader));
                }
            }
            return cardList;
        }
        public List<Card> GetCardsByIds(IEnumerable<string> ids)
        {
            var cardList = new List<Card>();
            if (!ids.Any()) 
                return cardList;

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();

                // Creates a string like: '$id0,$id1,$id2'
                var parameterNames = ids.Select((id, index) => $"$id{index}").ToArray();
                var IN_Clause = string.Join(",", parameterNames);

                command.CommandText = $"SELECT Id, Name, Colors, ManaCost, Cmc, TypeLine, [Set], Power, Toughness, Rarity, CollectorNumber, OracleText, Layout, ColorIdentity, " +
                                      $"SecondName, SecondManaCost, SecondTypeLine, SecondOracleText, SecondColors, SecondPower, SecondToughness " +
                                      $"FROM Cards WHERE Id IN ({IN_Clause})";

                for (int i = 0; i < parameterNames.Length; i++)
                    command.Parameters.AddWithValue(parameterNames[i], ids.ElementAt(i));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        cardList.Add(MapReaderToCard(reader)); // Helper method to keep it clean
                }
            }
            return cardList;
        }
        public List<Card> GetCardsByFilteredSearch(string filter, bool inName, bool inTypes, bool inRules)
        {
            List<Card> cardList = new List<Card>();

            // default to search by name if no boxes are checked
            if (!inName && !inTypes && !inRules)
                inName = true;

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();

                // dynamically build the WHERE clause
                List<string> filters = new List<string>();
                if (inName) filters.Add("Name LIKE $filter");
                if (inTypes) filters.Add("TypeLine LIKE $filter");
                if (inRules) filters.Add("OracleText LIKE $filter");
                string whereClause = string.Join(" OR ", filters);

                command.CommandText = $@"
                    SELECT Id, Name, Colors, ManaCost, Cmc, TypeLine, [Set], Power, 
                           Toughness, Rarity, CollectorNumber, OracleText, Layout, ColorIdentity, 
                           SecondName, SecondManaCost, SecondTypeLine, SecondOracleText, SecondColors, SecondPower, SecondToughness
                    FROM Cards 
                    WHERE ({whereClause})";
                command.Parameters.AddWithValue("$filter", $"%{filter}%");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        cardList.Add(MapReaderToCard(reader));
                }
            }
            return cardList;
        }
    }
}
