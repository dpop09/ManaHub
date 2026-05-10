using System.Collections.ObjectModel;

namespace ManaHub.Models
{
    class DeckGroup
    {
        public int ManaValue { get; set; }
        public required ObservableCollection<Card> Cards { get; set; }
    }
}
