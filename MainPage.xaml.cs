using System.Windows;
using System.Windows.Controls;

namespace HeavenUsurperToolkit
{
    public partial class MainPage : Page, IResizablePage
    {
        public double DesiredWidth => 800;
        public double DesiredHeight => 850;
        public MainPage()
        {
            InitializeComponent();
        }

        private void BtnToLN_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.NavigateToPage(new LNTransformer());
            }
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.ToggleParticleEffect();
                // mainWindow.NavigateToPage(new PatternGenerator());
            }
        }

        private void BtnToKeyConverter_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.NavigateToPage(new KeyConverter());
            }
        }

        private void BtnToLevel_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.NavigateToPage(new LevelCalculator());
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.ToggleParticleEffect();
            }
        }
    }
}