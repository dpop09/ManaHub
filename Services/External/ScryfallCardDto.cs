using System.Text.Json.Serialization;

namespace ManaHub.Services.External
{
    internal sealed class ScryfallCardDto
    {
        [JsonPropertyName("id")]
        public string? Id { get; init; }

        [JsonPropertyName("name")]
        public string? Name { get; init; }

        [JsonPropertyName("mana_cost")]
        public string? ManaCost { get; init; }

        [JsonPropertyName("cmc")]
        public double ManaValue { get; init; }

        [JsonPropertyName("type_line")]
        public string? TypeLine { get; init; }

        [JsonPropertyName("colors")]
        public List<string>? Colors { get; init; }

        [JsonPropertyName("color_identity")]
        public List<string>? ColorIdentity { get; init; }

        [JsonPropertyName("set")]
        public string? SetCode { get; init; }

        [JsonPropertyName("power")]
        public string? Power { get; init; }

        [JsonPropertyName("toughness")]
        public string? Toughness { get; init; }

        [JsonPropertyName("rarity")]
        public string? Rarity { get; init; }

        [JsonPropertyName("collector_number")]
        public string? CollectorNumber { get; init; }

        [JsonPropertyName("oracle_text")]
        public string? OracleText { get; init; }

        [JsonPropertyName("layout")]
        public string? Layout { get; init; }

        [JsonPropertyName("image_uris")]
        public ScryfallImageUrisDto? ImageUris { get; init; }

        [JsonPropertyName("card_faces")]
        public List<ScryfallCardFaceDto>? CardFaces { get; init; }
    }

    internal sealed class ScryfallCardFaceDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; init; }

        [JsonPropertyName("mana_cost")]
        public string? ManaCost { get; init; }

        [JsonPropertyName("type_line")]
        public string? TypeLine { get; init; }

        [JsonPropertyName("oracle_text")]
        public string? OracleText { get; init; }

        [JsonPropertyName("colors")]
        public List<string>? Colors { get; init; }

        [JsonPropertyName("power")]
        public string? Power { get; init; }

        [JsonPropertyName("toughness")]
        public string? Toughness { get; init; }

        [JsonPropertyName("image_uris")]
        public ScryfallImageUrisDto? ImageUris { get; init; }
    }

    internal sealed class ScryfallImageUrisDto
    {
        [JsonPropertyName("normal")]
        public string? Normal { get; init; }
    }
}
