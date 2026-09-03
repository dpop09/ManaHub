namespace ManaHub.Services.External
{
    internal sealed class DeckFileDto
    {
        public string? DeckName { get; init; }
        public List<string>? MainDeckIds { get; init; }
        public List<string>? SideboardIds { get; init; }
    }
}
