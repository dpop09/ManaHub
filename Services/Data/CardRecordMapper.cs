using ManaHub.Models;
using Microsoft.Data.Sqlite;

namespace ManaHub.Services
{
    internal static class CardRecordMapper
    {
        public const string SelectColumns = @"
            Id, Name, Colors, ManaCost, Cmc, TypeLine, [Set], Power,
            Toughness, Rarity, CollectorNumber, OracleText, Layout, ColorIdentity,
            PrimaryImageUrl, SecondName, SecondManaCost, SecondTypeLine, SecondOracleText,
            SecondColors, SecondPower, SecondToughness, SecondaryImageUrl";

        public static Card Map(SqliteDataReader reader)
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
                PrimaryImageUrl = reader.IsDBNull(14) ? "" : reader.GetString(14),
                SecondName = reader.IsDBNull(15) ? "" : reader.GetString(15),
                SecondManaCost = reader.IsDBNull(16) ? "" : reader.GetString(16),
                SecondTypeLine = reader.IsDBNull(17) ? "" : reader.GetString(17),
                SecondOracleText = reader.IsDBNull(18) ? "" : reader.GetString(18),
                SecondColorsString = reader.IsDBNull(19) ? "" : reader.GetString(19),
                SecondPower = reader.IsDBNull(20) ? "" : reader.GetString(20),
                SecondToughness = reader.IsDBNull(21) ? "" : reader.GetString(21),
                SecondaryImageUrl = reader.IsDBNull(22) ? "" : reader.GetString(22)
            };
        }
    }
}
