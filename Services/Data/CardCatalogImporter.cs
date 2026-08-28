using ManaHub.Contracts;
using ManaHub.Models;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Text.Json;

namespace ManaHub.Services
{
    internal sealed class CardCatalogImporter : ICardCatalogImporter
    {
        private static readonly HashSet<string> ForbiddenLayouts = new(
            new[] { "token", "double_faced_token", "emblem", "art_series", "planar" },
            StringComparer.OrdinalIgnoreCase);

        private static readonly HashSet<string> ForbiddenSets = new(
            new[] { "FJMP", "CMB2", "UNH", "HHO", "JTLA", "OARC", "SUNF", "FCLU", "OPCA", "FLTR", "MOC", "PVAN", "UNK" },
            StringComparer.OrdinalIgnoreCase);

        private readonly SqliteConnectionFactory _connections;

        public CardCatalogImporter(SqliteConnectionFactory connections)
        {
            _connections = connections;
        }

        public async Task ImportAsync(
            string filePath,
            CancellationToken cancellationToken = default)
        {
            using var stream = File.OpenRead(filePath);
            var cards = JsonSerializer.DeserializeAsyncEnumerable<Card>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                cancellationToken);

            using var connection = _connections.CreateConnection();
            await connection.OpenAsync(cancellationToken);
            using var transaction = connection.BeginTransaction();

            var command = CreateInsertCommand(connection, transaction);
            await foreach (var card in cards.WithCancellation(cancellationToken))
            {
                if (card == null || ShouldSkip(card))
                    continue;

                BindCard(command, card);
                await command.ExecuteNonQueryAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
        }

        private static SqliteCommand CreateInsertCommand(
            SqliteConnection connection,
            SqliteTransaction transaction)
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = @"
                INSERT INTO Cards (
                    Id, Name, Colors, ManaCost, Cmc, TypeLine, [Set], Power, Toughness,
                    Rarity, CollectorNumber, OracleText, Layout, ColorIdentity, SecondName,
                    SecondManaCost, SecondTypeLine, SecondOracleText, SecondColors,
                    SecondPower, SecondToughness, PrimaryImageUrl, SecondaryImageUrl)
                VALUES (
                    $id, $name, $colors, $mana, $cmc, $type, $set, $power, $tough,
                    $rarity, $collectorNumber, $oracleText, $layout, $colorIdentity,
                    $secondName, $secondMana, $secondType, $secondText, $secondColors,
                    $secondPower, $secondToughness, $primaryUrl, $secondaryUrl)";

            AddParameter(command, "$id", SqliteType.Text);
            AddParameter(command, "$name", SqliteType.Text);
            AddParameter(command, "$colors", SqliteType.Text);
            AddParameter(command, "$mana", SqliteType.Text);
            AddParameter(command, "$cmc", SqliteType.Real);
            AddParameter(command, "$type", SqliteType.Text);
            AddParameter(command, "$set", SqliteType.Text);
            AddParameter(command, "$power", SqliteType.Text);
            AddParameter(command, "$tough", SqliteType.Text);
            AddParameter(command, "$rarity", SqliteType.Text);
            AddParameter(command, "$collectorNumber", SqliteType.Text);
            AddParameter(command, "$oracleText", SqliteType.Text);
            AddParameter(command, "$layout", SqliteType.Text);
            AddParameter(command, "$colorIdentity", SqliteType.Text);
            AddParameter(command, "$secondName", SqliteType.Text);
            AddParameter(command, "$secondMana", SqliteType.Text);
            AddParameter(command, "$secondType", SqliteType.Text);
            AddParameter(command, "$secondText", SqliteType.Text);
            AddParameter(command, "$secondColors", SqliteType.Text);
            AddParameter(command, "$secondPower", SqliteType.Text);
            AddParameter(command, "$secondToughness", SqliteType.Text);
            AddParameter(command, "$primaryUrl", SqliteType.Text);
            AddParameter(command, "$secondaryUrl", SqliteType.Text);

            return command;
        }

        private static void AddParameter(SqliteCommand command, string name, SqliteType type)
        {
            command.Parameters.Add(name, type);
        }

        private static bool ShouldSkip(Card card)
        {
            return ForbiddenLayouts.Contains(card.Layout ?? string.Empty)
                || ForbiddenSets.Contains(card.Set ?? string.Empty);
        }

        private static void BindCard(SqliteCommand command, Card card)
        {
            Set(command, "$id", card.Id);
            Set(command, "$cmc", card.Cmc);
            Set(command, "$set", card.Set?.ToUpperInvariant());
            Set(command, "$rarity", FormatRarity(card.Rarity));
            Set(command, "$collectorNumber", card.CollectorNumber);
            Set(command, "$layout", card.Layout);
            Set(command, "$colorIdentity", JoinColors(card.ColorIdentity));

            if (card.CardFaces is { Count: >= 2 })
            {
                BindMultiFaceCard(command, card, card.CardFaces[0], card.CardFaces[1]);
                return;
            }

            BindSingleFaceCard(command, card);
        }

        private static void BindMultiFaceCard(
            SqliteCommand command,
            Card card,
            CardFace front,
            CardFace back)
        {
            Set(command, "$name", front.Name);
            Set(command, "$colors", JoinColors(card.Colors) ?? JoinColors(front.Colors));
            Set(command, "$mana", front.ManaCost);
            Set(command, "$type", front.TypeLine);
            Set(command, "$power", front.Power);
            Set(command, "$tough", front.Toughness);
            Set(command, "$oracleText", front.OracleText);
            Set(command, "$primaryUrl", front.ImageUris?.Normal);

            Set(command, "$secondName", back.Name);
            Set(command, "$secondMana", back.ManaCost);
            Set(command, "$secondType", back.TypeLine);
            Set(command, "$secondText", back.OracleText);
            Set(command, "$secondColors", JoinColors(back.Colors));
            Set(command, "$secondPower", back.Power);
            Set(command, "$secondToughness", back.Toughness);
            Set(command, "$secondaryUrl", back.ImageUris?.Normal);
        }

        private static void BindSingleFaceCard(SqliteCommand command, Card card)
        {
            Set(command, "$name", card.Name);
            Set(command, "$colors", JoinColors(card.Colors));
            Set(command, "$mana", card.ManaCost);
            Set(command, "$type", card.TypeLine);
            Set(command, "$power", card.Power);
            Set(command, "$tough", card.Toughness);
            Set(command, "$oracleText", card.OracleText);
            Set(command, "$primaryUrl", card.ImageUris?.Normal);

            Set(command, "$secondName", null);
            Set(command, "$secondMana", null);
            Set(command, "$secondType", null);
            Set(command, "$secondText", null);
            Set(command, "$secondColors", null);
            Set(command, "$secondPower", null);
            Set(command, "$secondToughness", null);
            Set(command, "$secondaryUrl", null);
        }

        private static string? JoinColors(IEnumerable<string>? colors)
        {
            return colors?.Any() == true ? string.Join(",", colors) : null;
        }

        private static string? FormatRarity(string? rarity)
        {
            if (string.IsNullOrWhiteSpace(rarity))
                return null;

            return char.ToUpperInvariant(rarity[0]) + rarity[1..].ToLowerInvariant();
        }

        private static void Set(SqliteCommand command, string parameterName, object? value)
        {
            command.Parameters[parameterName].Value = value ?? DBNull.Value;
        }
    }
}
