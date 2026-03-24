using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Windows;

namespace ManaHub.Services
{
    internal static class CardImageService
    {
        private static readonly string CacheDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ManaHub", "Cache");

        // Use ONLY this client. Do not create new ones in methods.
        private static readonly HttpClient _httpClient = new HttpClient(new HttpClientHandler
        {
            // Helps with SSL handshake issues in some Windows environments
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        });

        // Keeps track of what is currently downloading to prevent duplicate SSL requests
        private static readonly ConcurrentDictionary<string, Task> _activeDownloads = new();

        // Limit to 3 concurrent downloads. This prevents the firewall from 
        // seeing a "spike" that looks like an attack.
        private static readonly SemaphoreSlim _networkLocker = new SemaphoreSlim(3);

        static CardImageService()
        {
            // Force modern security protocols
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls13;

            _httpClient.DefaultRequestHeaders.Add("User-Agent", "ManaHub/1.0 (MTG Deckbuilder Project)");
            _httpClient.Timeout = TimeSpan.FromSeconds(15);
        }

        public static Uri GetImagePath(string scryfallId, string remoteUri, Action onDownloadComplete)
        {
            if (string.IsNullOrEmpty(remoteUri)) return null;
            if (!Directory.Exists(CacheDirectory)) Directory.CreateDirectory(CacheDirectory);

            string localFile = Path.Combine(CacheDirectory, $"{scryfallId}.jpg");

            if (File.Exists(localFile))
            {
                return new Uri(localFile, UriKind.Absolute);
            }

            // If not already downloading, start it
            if (!_activeDownloads.ContainsKey(scryfallId))
            {
                var task = DownloadAndCacheImageAsync(remoteUri, localFile, scryfallId, onDownloadComplete);
                _activeDownloads.TryAdd(scryfallId, task);
            }

            return new Uri(remoteUri, UriKind.Absolute);
        }

        private static async Task DownloadAndCacheImageAsync(string uri, string localPath, string id, Action callback)
        {
            // Wait for a spot to open up in the "queue"
            await _networkLocker.WaitAsync();

            try
            {
                // Optional: tiny staggered start
                await Task.Delay(50);

                byte[] data = await _httpClient.GetByteArrayAsync(uri);
                await File.WriteAllBytesAsync(localPath, data);

                Application.Current.Dispatcher.Invoke(() => callback?.Invoke());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ImageService] Error: {ex.Message}");
            }
            finally
            {
                _activeDownloads.TryRemove(id, out _);
                // Release the spot so the next image can start
                _networkLocker.Release();
            }
        }
    }
}