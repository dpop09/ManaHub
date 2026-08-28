using ManaHub.Contracts;
using Microsoft.Win32;
using System.Windows;

namespace ManaHub.Services
{
    internal sealed class WpfDialogService : IDialogService
    {
        public void ShowMessage(string message, string title = "Notification")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK);
        }

        public bool Confirm(string message, string title)
        {
            return MessageBox.Show(message, title, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }
    }

    internal sealed class WpfFileDialogService : IFileDialogService
    {
        private const string DeckFilter = "ManaHub Deck (*.json)|*.json";

        public string? SelectDeckToOpen()
        {
            var dialog = new OpenFileDialog { Filter = DeckFilter };
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public string? SelectDeckToSave()
        {
            var dialog = new SaveFileDialog { Filter = DeckFilter };
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }
    }

    internal sealed class WpfWindowService : IWindowService
    {
        public void Minimize()
        {
            if (Application.Current.MainWindow is { } window)
                window.WindowState = WindowState.Minimized;
        }

        public void ToggleMaximize()
        {
            if (Application.Current.MainWindow is not { } window)
                return;

            window.WindowState = window.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        public void Close() => Application.Current.Shutdown();
    }
}
