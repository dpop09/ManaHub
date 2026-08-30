using System.Windows.Controls;

namespace ManaHub.Views
{
    /// <summary>
    /// Interaction logic for CardDisplay.xaml
    /// </summary>
    public partial class CardDisplay : UserControl
    {
        public CardDisplay()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private async void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is ViewModels.CardDisplayViewModel viewModel)
                await viewModel.ActivateAsync();
        }

        private void OnUnloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is ViewModels.CardDisplayViewModel viewModel)
                viewModel.Deactivate();
        }
    }
}
