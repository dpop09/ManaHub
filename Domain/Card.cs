namespace ManaHub.Domain
{
    public sealed class Card
    {
        public Card(
            string id,
            double manaValue,
            string setCode,
            string rarity,
            string collectorNumber,
            string layout,
            IEnumerable<string> colorIdentity,
            IEnumerable<CardFace> faces)
        {
            Id = id;
            ManaValue = manaValue;
            SetCode = setCode;
            Rarity = rarity;
            CollectorNumber = collectorNumber;
            Layout = layout;
            ColorIdentity = colorIdentity.ToArray();
            Faces = faces.ToArray();

            if (Faces.Count == 0)
                throw new ArgumentException("A card must have at least one face.", nameof(faces));
        }

        public string Id { get; }
        public double ManaValue { get; }
        public string SetCode { get; }
        public string Rarity { get; }
        public string CollectorNumber { get; }
        public string Layout { get; }
        public IReadOnlyList<string> ColorIdentity { get; }
        public IReadOnlyList<CardFace> Faces { get; }

        public CardFace Front => Faces[0];
        public CardFace? Back => Faces.Count > 1 ? Faces[1] : null;

        public string Name => CombineFaces(face => face.Name);
        public string TypeLine => CombineFaces(face => face.TypeLine);
        public string ManaCost => CombineFaces(face => face.ManaCost);

        private string CombineFaces(Func<CardFace, string> selector)
        {
            return Back == null
                ? selector(Front)
                : $"{selector(Front)} // {selector(Back)}";
        }
    }

    public sealed class CardFace
    {
        public CardFace(
            string name,
            string manaCost,
            string typeLine,
            string oracleText,
            IEnumerable<string> colors,
            string power,
            string toughness,
            string imageUrl)
        {
            Name = name;
            ManaCost = manaCost;
            TypeLine = typeLine;
            OracleText = oracleText;
            Colors = colors.ToArray();
            Power = power;
            Toughness = toughness;
            ImageUrl = imageUrl;
        }

        public string Name { get; }
        public string ManaCost { get; }
        public string TypeLine { get; }
        public string OracleText { get; }
        public IReadOnlyList<string> Colors { get; }
        public string Power { get; }
        public string Toughness { get; }
        public string ImageUrl { get; }
    }
}
