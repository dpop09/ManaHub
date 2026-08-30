using System.IO;
using System.Text.Json;

using ManaHub.Contracts;
using ManaHub.Domain;
using ManaHub.Services.External;

namespace ManaHub.Services
{
    internal sealed class DeckService : IDeckService
    {
        public async Task SaveToFileAsync(
            string path,
            DeckDocument document,
            CancellationToken cancellationToken = default)
        {
            var data = new DeckFileDto
            {
                DeckName = document.Name,
                MainDeckIds = document.MainDeckIds.ToList(),
                SideboardIds = document.SideboardIds.ToList()
            };

            string json = JsonSerializer.Serialize(data);
            await File.WriteAllTextAsync(path, json, cancellationToken);
        }

        public async Task<DeckDocument> LoadFromFileAsync(
            string path,
            CancellationToken cancellationToken = default)
        {
            string json = await File.ReadAllTextAsync(path, cancellationToken);
            var data = JsonSerializer.Deserialize<DeckFileDto>(json)
                ?? throw new InvalidDataException("The selected deck file is invalid.");

            if (data.MainDeckIds == null || data.SideboardIds == null)
                throw new InvalidDataException("The selected deck file does not contain valid card lists.");

            return new DeckDocument(
                data.DeckName ?? string.Empty,
                data.MainDeckIds,
                data.SideboardIds);
        }
    }
}
