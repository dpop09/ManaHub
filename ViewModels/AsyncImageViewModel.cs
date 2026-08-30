using ManaHub.Contracts;
using ManaHub.MVVMs;

namespace ManaHub.ViewModels
{
    internal sealed class AsyncImageViewModel : ViewModelBase, IDisposable
    {
        private readonly ICardImageService _images;
        private CancellationTokenSource? _requestCancellation;
        private Uri? _source;
        private bool _isLoading;
        private string? _errorMessage;
        private long _requestVersion;

        public AsyncImageViewModel(ICardImageService images)
        {
            _images = images;
        }

        public Uri? Source
        {
            get => _source;
            private set
            {
                if (_source == value)
                    return;

                _source = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                if (_isLoading == value)
                    return;

                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public string? ErrorMessage
        {
            get => _errorMessage;
            private set
            {
                if (_errorMessage == value)
                    return;

                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public void StartLoad(string cacheKey, string remoteUri)
        {
            _ = LoadAsync(cacheKey, remoteUri);
        }

        public async Task LoadAsync(
            string cacheKey,
            string remoteUri,
            CancellationToken cancellationToken = default)
        {
            long requestVersion = Interlocked.Increment(ref _requestVersion);
            var requestCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var previousRequest = Interlocked.Exchange(ref _requestCancellation, requestCancellation);
            previousRequest?.Cancel();

            Source = null;
            ErrorMessage = null;
            IsLoading = true;

            try
            {
                Uri? source = await _images.GetImageAsync(
                    cacheKey,
                    remoteUri,
                    requestCancellation.Token);

                if (IsCurrent(requestVersion, requestCancellation))
                    Source = source;
            }
            catch (OperationCanceledException) when (requestCancellation.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                if (IsCurrent(requestVersion, requestCancellation))
                    ErrorMessage = ex.Message;
            }
            finally
            {
                if (IsCurrent(requestVersion, requestCancellation))
                {
                    Interlocked.CompareExchange(ref _requestCancellation, null, requestCancellation);
                    IsLoading = false;
                }

                requestCancellation.Dispose();
            }
        }

        public void Clear()
        {
            Interlocked.Increment(ref _requestVersion);
            var activeRequest = Interlocked.Exchange(ref _requestCancellation, null);
            activeRequest?.Cancel();
            Source = null;
            ErrorMessage = null;
            IsLoading = false;
        }

        public void Dispose()
        {
            Clear();
        }

        private bool IsCurrent(long requestVersion, CancellationTokenSource requestCancellation)
        {
            return requestVersion == Interlocked.Read(ref _requestVersion)
                && ReferenceEquals(_requestCancellation, requestCancellation)
                && !requestCancellation.IsCancellationRequested;
        }
    }
}
