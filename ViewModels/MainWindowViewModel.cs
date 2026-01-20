using ManaHub.MVVMs;
using ManaHub.Services;
using System.IO;
using System.Windows;

namespace ManaHub.ViewModels
{
    // MainWindowViewModel acts as the "mediator" for views to communicate indirectly when page navigation occurs
    internal class MainWindowViewModel : ViewModelBase
    {
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set 
            { 
                _currentView = value; 
                OnPropertyChanged();
                // whenever the view changes, reevaluate the need to show the nav bar
                UpdateNavVisibility();
            }
        }
        private Visibility _navVisibility = Visibility.Collapsed;
        public Visibility NavVisibility
        {
            get => _navVisibility;
            set
            {
                _navVisibility = value;
                OnPropertyChanged();
            }
        }
        public NavigationBarViewModel NavVM { get; set; }
        public RelayCommand ShowGoToCreateAccountPageCommand {  get; set; }
        public RelayCommand CloseWindowCommand { get; set; }
        public RelayCommand MinimizeWindowCommand { get; set; }
        public RelayCommand MaximizeWindowCommand { get; set; }

        public MainWindowViewModel()
        {
            InitializeApp();

            // Commands to swap the view
            ShowGoToCreateAccountPageCommand = new RelayCommand(o => CurrentView = new CreateAccountPageViewModel(this));
            MinimizeWindowCommand = new RelayCommand(o => MinimizeWindow());
            MaximizeWindowCommand = new RelayCommand(o => MaximizeWindow());
            CloseWindowCommand = new RelayCommand(o => CloseWindow());
        }

        private void UpdateNavVisibility()
        {
            // make the nav bar invisibile when user is not logged in
            if (CurrentView is LoginPageViewModel || CurrentView is CreateAccountPageViewModel)
                NavVisibility = Visibility.Collapsed;
            else
                NavVisibility = Visibility.Visible;
        }
        private void MinimizeWindow()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
        private void MaximizeWindow() 
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            else
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
        }
        private void CloseWindow()
        {
            Application.Current.Shutdown();
        }
        private async void InitializeApp()
        {
            var db = DatabaseService.Instance;
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "oracle-cards-20260117221532.json");

            if (File.Exists(filePath))
            {
                if (db.GetCardCount() == 0)
                {
                    await Task.Run(async () =>
                    {
                        await db.BulkImportCards(filePath);
                    });
                }
            }
            else
            {
                Console.WriteLine($"Critical Error: File not found at {filePath}");
            }
            NavVM = new NavigationBarViewModel(this);
            CurrentView = new LoginPageViewModel(this);
        }
    }
}
