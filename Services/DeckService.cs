using ManaHub.Models;
using System.IO;
using System.Text.Json;

using ManaHub.Contracts;

namespace ManaHub.Services
{
    internal sealed class DeckService : IDeckService
    {
        public async Task SaveToFileAsync(
            string path,
            string name,
            IEnumerable<Card> main,
            IEnumerable<Card> side,
            CancellationToken cancellationToken = default)
        {
            var data = new DeckSaveModel
            {
                DeckName = name,
                MainDeckIds = main.Select(c => c.Id).ToList(),
                SideboardIds = side.Select(c => c.Id).ToList()
            };
            string json = JsonSerializer.Serialize(data);
            await File.WriteAllTextAsync(path, json, cancellationToken);
        }

        public async Task<DeckSaveModel> LoadFromFileAsync(
            string path,
            CancellationToken cancellationToken = default)
        {
            string json = await File.ReadAllTextAsync(path, cancellationToken);
            return JsonSerializer.Deserialize<DeckSaveModel>(json)
                ?? throw new InvalidDataException("The selected deck file is invalid.");
        }
    }
}
