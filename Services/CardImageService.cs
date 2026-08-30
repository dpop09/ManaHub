using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Windows;

using ManaHub.Contracts;

namespace ManaHub.Services
{
    internal sealed class CardImageService : ICardImageService
    {
        private readonly HttpClient _httpClient;
        private readonly string _cacheDirectory;
        private readonly ConcurrentDictionary<string, Task> _activeDownloads = new();
        private readonly SemaphoreSlim _networkLocker = new(3);

        public CardImageService(HttpClient httpClient, string cacheDirectory)
        {
            _httpClient = httpClient;
            _cacheDirectory = cacheDirectory;
        }

        public Uri? GetImagePath(string cacheKey, string remoteUri, Action onDownloadComplete)
        {
            if (string.IsNullOrEmpty(remoteUri))
                return null;

            Directory.CreateDirectory(_cacheDirectory);

            string localFile = Path.Combine(_cacheDirectory, $"{cacheKey}.jpg");

            if (File.Exists(localFile))
            {
                return new Uri(localFile, UriKind.Absolute);
            }

            _activeDownloads.GetOrAdd(
                cacheKey,
                _ => DownloadAndCacheImageAsync(remoteUri, localFile, cacheKey, onDownloadComplete));

            return new Uri(remoteUri, UriKind.Absolute);
        }

        private async Task DownloadAndCacheImageAsync(
            string uri,
            string localPath,
            string cacheKey,
            Action callback)
        {
            await _networkLocker.WaitAsync();

            try
            {
                byte[] data = await _httpClient.GetByteArrayAsync(uri);
                await File.WriteAllBytesAsync(localPath, data);

                Application.Current.Dispatcher.Invoke(callback);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ImageService] Error: {ex.Message}");
            }
            finally
            {
                _activeDownloads.TryRemove(cacheKey, out _);
                _networkLocker.Release();
            }
        }
    }
}
