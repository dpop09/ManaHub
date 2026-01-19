using System.Text.Json.Serialization;

namespace ManaHub.Models
{
    public class Card
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("mana_cost")]
        public string ManaCost { get; set; }

        [JsonPropertyName("type_line")]
        public string TypeLine { get; set; }

        [JsonPropertyName("oracle_text")]
        public string OracleText { get; set; }
    }
}
