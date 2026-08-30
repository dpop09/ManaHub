namespace ManaHub.Domain
{
    public sealed class DeckDocument
    {
        public DeckDocument(
            string name,
            IEnumerable<string> mainDeckIds,
            IEnumerable<string> sideboardIds)
        {
            Name = name;
            MainDeckIds = mainDeckIds.ToArray();
            SideboardIds = sideboardIds.ToArray();
        }

        public string Name { get; }
        public IReadOnlyList<string> MainDeckIds { get; }
        public IReadOnlyList<string> SideboardIds { get; }
    }
}
