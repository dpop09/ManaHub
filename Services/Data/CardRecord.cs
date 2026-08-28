namespace ManaHub.Services
{
    internal sealed class CardRecord
    {
        public string Id { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string Colors { get; init; } = string.Empty;
        public string ManaCost { get; init; } = string.Empty;
        public double ManaValue { get; init; }
        public string TypeLine { get; init; } = string.Empty;
        public string SetCode { get; init; } = string.Empty;
        public string Power { get; init; } = string.Empty;
        public string Toughness { get; init; } = string.Empty;
        public string Rarity { get; init; } = string.Empty;
        public string CollectorNumber { get; init; } = string.Empty;
        public string OracleText { get; init; } = string.Empty;
        public string Layout { get; init; } = string.Empty;
        public string ColorIdentity { get; init; } = string.Empty;
        public string PrimaryImageUrl { get; init; } = string.Empty;
        public string SecondName { get; init; } = string.Empty;
        public string SecondManaCost { get; init; } = string.Empty;
        public string SecondTypeLine { get; init; } = string.Empty;
        public string SecondOracleText { get; init; } = string.Empty;
        public string SecondColors { get; init; } = string.Empty;
        public string SecondPower { get; init; } = string.Empty;
        public string SecondToughness { get; init; } = string.Empty;
        public string SecondaryImageUrl { get; init; } = string.Empty;
    }
}
