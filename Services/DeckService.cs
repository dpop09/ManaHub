using ManaHub.Models;
using System.IO;
using System.Text.Json;

using ManaHub.Contracts;

namespace ManaHub.Services
{
    internal sealed class DeckService : IDeckService
    {
        public void SaveToFile(string path, string name, IEnumerable<Card> main, IEnumerable<Card> side)
        {
            var data = new DeckSaveModel
            {
                DeckName = name,
                MainDeckIds = main.Select(c => c.Id).ToList(),
                SideboardIds = side.Select(c => c.Id).ToList()
            };
            string json = JsonSerializer.Serialize(data);
            File.WriteAllText(path, json);
        }

        public DeckSaveModel LoadFromFile(string path)
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<DeckSaveModel>(json)
                ?? throw new InvalidDataException("The selected deck file is invalid.");
        }
    }
}
