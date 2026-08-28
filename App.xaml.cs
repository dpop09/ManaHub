using System.Configuration;
using System.Windows;

namespace ManaHub
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private CompositionRoot? _compositionRoot;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                _compositionRoot = new CompositionRoot();
                await _compositionRoot.InitializeAsync();

                MainWindow = await _compositionRoot.CreateMainWindowAsync();
                MainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"ManaHub could not start.\n\n{ex.Message}",
                    "Startup Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown(-1);
            }
        }
    }

}
