using System.Text.Json.Serialization;

namespace ManaHub.Models
{
    public class Card
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("mana_cost")]
        public string ManaCost { get; set; }

        [JsonPropertyName("cmc")]
        public double Cmc {  get; set; }

        [JsonPropertyName("type_line")]
        public string TypeLine { get; set; }

        [JsonPropertyName("colors")]
        public List<string> Colors { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public string ColorsString { get; set; }

        [JsonPropertyName("set")]
        public string Set { get; set; }

        [JsonPropertyName("power")]
        public string Power { get; set; }

        [JsonPropertyName("toughness")]
        public string Toughness { get; set; }

        [JsonPropertyName("rarity")]
        public string Rarity { get; set; }

        [JsonPropertyName("collector_number")]
        public string CollectorNumber { get; set; }

        [JsonPropertyName("oracle_text")]
        public string OracleText { get; set; }

        [JsonPropertyName("layout")]
        public string Layout { get; set; }
    }
}
