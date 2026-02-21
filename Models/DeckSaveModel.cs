namespace ManaHub.Models
{
    public class DeckSaveModel
    {
        public string DeckName { get; set; }
        public List<string> MainDeckIds { get; set; }
        public List<string> SideboardIds { get; set; }
    }
}
