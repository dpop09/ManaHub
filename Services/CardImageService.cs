using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;

using ManaHub.Contracts;

namespace ManaHub.Services
{
    internal sealed class CardImageService : ICardImageService
    {
        private readonly HttpClient _httpClient;
        private readonly string _cacheDirectory;
        private readonly ConcurrentDictionary<string, Lazy<SharedDownload>> _activeDownloads = new();
        private readonly SemaphoreSlim _networkGate = new(3);

        public CardImageService(HttpClient httpClient, string cacheDirectory)
        {
            _httpClient = httpClient;
            _cacheDirectory = cacheDirectory;
        }

        public async Task<Uri?> GetImageAsync(
            string cacheKey,
            string remoteUri,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(remoteUri))
                return null;

            ValidateCacheKey(cacheKey);
            string localPath = Path.Combine(_cacheDirectory, $"{cacheKey}.jpg");
            bool isCached = await Task.Run(() =>
            {
                Directory.CreateDirectory(_cacheDirectory);
                return File.Exists(localPath);
            }, cancellationToken).ConfigureAwait(false);
            if (isCached)
                return new Uri(localPath, UriKind.Absolute);

            Lazy<SharedDownload> pendingDownload;
            SharedDownload sharedDownload;
            while (true)
            {
                pendingDownload = _activeDownloads.GetOrAdd(
                    cacheKey,
                    _ => new Lazy<SharedDownload>(
                        () => new SharedDownload(
                            token => DownloadAndCacheAsync(remoteUri, localPath, token)),
                        LazyThreadSafetyMode.ExecutionAndPublication));
                sharedDownload = pendingDownload.Value;
                if (sharedDownload.TryAddWaiter())
                    break;

                RemoveDownload(cacheKey, pendingDownload);
            }

            try
            {
                return await sharedDownload.Task
                    .WaitAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
            finally
            {
                sharedDownload.ReleaseWaiter();
                if (sharedDownload.CanRemove)
                    RemoveDownload(cacheKey, pendingDownload);
            }
        }

        private async Task<Uri?> DownloadAndCacheAsync(
            string remoteUri,
            string localPath,
            CancellationToken cancellationToken)
        {
            await _networkGate.WaitAsync(cancellationToken).ConfigureAwait(false);
            string temporaryPath = $"{localPath}.{Guid.NewGuid():N}.tmp";

            try
            {
                if (File.Exists(localPath))
                    return new Uri(localPath, UriKind.Absolute);

                using var response = await _httpClient.GetAsync(
                    remoteUri,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                await using var source = await response.Content
                    .ReadAsStreamAsync(cancellationToken)
                    .ConfigureAwait(false);
                await using (var destination = new FileStream(
                    temporaryPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 81920,
                    useAsync: true))
                {
                    await source.CopyToAsync(destination, cancellationToken).ConfigureAwait(false);
                }

                File.Move(temporaryPath, localPath, overwrite: true);
                return new Uri(localPath, UriKind.Absolute);
            }
            finally
            {
                try
                {
                    if (File.Exists(temporaryPath))
                        File.Delete(temporaryPath);
                }
                finally
                {
                    _networkGate.Release();
                }
            }
        }

        private void RemoveDownload(
            string cacheKey,
            Lazy<SharedDownload> pendingDownload)
        {
            var entry = new KeyValuePair<string, Lazy<SharedDownload>>(cacheKey, pendingDownload);
            ((ICollection<KeyValuePair<string, Lazy<SharedDownload>>>)_activeDownloads).Remove(entry);
        }

        private static void ValidateCacheKey(string cacheKey)
        {
            if (string.IsNullOrWhiteSpace(cacheKey)
                || cacheKey.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new ArgumentException("The image cache key is invalid.", nameof(cacheKey));
            }
        }

        private sealed class SharedDownload
        {
            private readonly object _gate = new();
            private readonly CancellationTokenSource _cancellation = new();
            private int _waiterCount;
            private bool _acceptingWaiters = true;

            public SharedDownload(Func<CancellationToken, Task<Uri?>> download)
            {
                Task = download(_cancellation.Token);
            }

            public Task<Uri?> Task { get; }

            public bool CanRemove
            {
                get
                {
                    lock (_gate)
                        return !_acceptingWaiters;
                }
            }

            public bool TryAddWaiter()
            {
                lock (_gate)
                {
                    if (!_acceptingWaiters)
                        return false;

                    _waiterCount++;
                    return true;
                }
            }

            public void ReleaseWaiter()
            {
                bool cancelDownload = false;
                lock (_gate)
                {
                    _waiterCount--;
                    if (Task.IsCompleted)
                        _acceptingWaiters = false;
                    else if (_waiterCount == 0)
                    {
                        _acceptingWaiters = false;
                        cancelDownload = true;
                    }
                }

                if (cancelDownload)
                    _cancellation.Cancel();
            }
        }
    }
}
