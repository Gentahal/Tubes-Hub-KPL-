using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TubesHubGUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainContentArea.Content = new FinalisasiView();
        }

        private void NavFinalisasi_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new FinalisasiView();
        }

        private void NavWbs_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new WBSView();
        }

        private void NavProgress_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new ProgressView();
        }

        private void NavTeam_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new TextBlock { Text = "Halaman Team Info sedang dikerjakan...", FontSize = 18, Margin = new Avalonia.Thickness(20) };
        }

        private void NavScheduler_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new SchedulerView();
        }
    }
}