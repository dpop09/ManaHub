using System.Windows.Input;

namespace ManaHub.MVVMs
{
    internal sealed class AsyncRelayCommand : ICommand
    {
        private readonly Func<object?, CancellationToken, Task> _execute;
        private readonly Func<object?, bool>? _canExecute;
        private CancellationTokenSource? _executionCancellation;
        private bool _isExecuting;

        public AsyncRelayCommand(
            Func<object?, CancellationToken, Task> execute,
            Func<object?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);
        }

        public async void Execute(object? parameter)
        {
            await ExecuteAsync(parameter);
        }

        public async Task ExecuteAsync(object? parameter = null)
        {
            if (!CanExecute(parameter))
                return;

            _isExecuting = true;
            _executionCancellation = new CancellationTokenSource();
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);

            try
            {
                await _execute(parameter, _executionCancellation.Token);
            }
            finally
            {
                _executionCancellation.Dispose();
                _executionCancellation = null;
                _isExecuting = false;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Cancel() => _executionCancellation?.Cancel();
    }
}
