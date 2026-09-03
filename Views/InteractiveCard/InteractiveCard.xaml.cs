using System.Windows.Controls;

namespace ManaHub.Views
{
    /// <summary>
    /// Interaction logic for InteractiveCard.xaml
    /// </summary>
    public partial class InteractiveCard : UserControl
    {
        private ViewModels.InteractiveCardViewModel? _activeViewModel;

        public InteractiveCard()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            DataContextChanged += OnDataContextChanged;
        }

        private async void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            await ActivateDataContextAsync();
        }

        private void OnUnloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            DeactivateCurrentViewModel();
        }

        private async void OnDataContextChanged(
            object sender,
            System.Windows.DependencyPropertyChangedEventArgs e)
        {
            DeactivateCurrentViewModel();
            if (IsLoaded)
                await ActivateDataContextAsync();
        }

        private async Task ActivateDataContextAsync()
        {
            if (DataContext is not ViewModels.InteractiveCardViewModel viewModel)
                return;

            _activeViewModel = viewModel;
            await viewModel.ActivateAsync();
        }

        private void DeactivateCurrentViewModel()
        {
            _activeViewModel?.Deactivate();
            _activeViewModel = null;
        }
    }
}
