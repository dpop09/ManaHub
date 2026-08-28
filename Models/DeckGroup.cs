using System.Collections.ObjectModel;

using ManaHub.Domain;

namespace ManaHub.Models
{
    class DeckGroup
    {
        public int ManaValue { get; set; }
        public required ObservableCollection<Card> Cards { get; set; }
    }
}
