using System.Text.Json.Serialization;

namespace ManaHub.Models
{
    public class Card
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
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
        [JsonPropertyName("color_identity")]
        public List<string> ColorIdentity { get; set; }
        public string ColorIdentityString { get; set; }

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

        public string SecondName { get; set; }
        public string SecondManaCost { get; set; }
        public string SecondTypeLine { get; set; }
        public string SecondOracleText { get; set; }
        public List<string> SecondColors { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public string SecondColorsString { get; set; }
        public string SecondPower { get; set; }
        public string SecondToughness { get; set; }

        [JsonPropertyName("card_faces")]
        public List<CardFace> CardFaces { get; set; }

        public string RowDisplayName 
        { 
            get
            {
                if (!string.IsNullOrEmpty(SecondName))
                    return $"{Name} // {SecondName}";
                return Name;
            }
        }
        public string RowDisplayTypeLine
        {
            get
            {
                if (!string.IsNullOrEmpty(SecondTypeLine))
                    return $"{TypeLine} // {SecondTypeLine}";
                return TypeLine;
            }
        }
        public string RowDisplayManaCost
        {
            get
            {
                if (!string.IsNullOrEmpty(SecondManaCost))
                    return $"{ManaCost} // {SecondManaCost}";
                return ManaCost;
            }
        }
    }

    public class CardFace
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("mana_cost")]
        public string ManaCost { get; set; }

        [JsonPropertyName("type_line")]
        public string TypeLine { get; set; }

        [JsonPropertyName("oracle_text")]
        public string OracleText { get; set; }

        [JsonPropertyName("colors")]
        public List<string> Colors { get; set; }

        [JsonPropertyName("power")]
        public string Power { get; set; }

        [JsonPropertyName("toughness")]
        public string Toughness { get; set; }
    }
}
