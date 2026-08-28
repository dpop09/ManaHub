using ManaHub.MVVMs;
using System.Windows.Input;

namespace ManaHub.ViewModels
{
    internal class SettingsPageViewModel : ViewModelBase
    {
        public ICommand ClearCardImagesCacheCommand { get; }
        public ICommand DeleteCardDatabaseCommand { get; }

        public SettingsPageViewModel()
        {
            ClearCardImagesCacheCommand = new RelayCommand((o) => ClearCardImagesCache());
            DeleteCardDatabaseCommand = new RelayCommand((o) => DeleteCardDatabase());
        }

        private void ClearCardImagesCache()
        {
            Console.WriteLine("Cache is cleared.");
        }

        private void DeleteCardDatabase()
        {
            Console.WriteLine("Card database is deleted.");
        }
    }
}
