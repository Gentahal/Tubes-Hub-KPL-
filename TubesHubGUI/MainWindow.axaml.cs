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
            MainContentArea.Content = new TextBlock { Text = "Halaman WBS milik Ahmad sedang dikerjakan...", FontSize = 18, Margin = new Avalonia.Thickness(20) };
        }

        private void NavProgress_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new TextBlock { Text = "Halaman Progress milik Aufa sedang dikerjakan...", FontSize = 18, Margin = new Avalonia.Thickness(20) };
        }

        private void NavTeam_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new TextBlock { Text = "Halaman Team Info milik Zaidan sedang dikerjakan...", FontSize = 18, Margin = new Avalonia.Thickness(20) };
        }

        private void NavScheduler_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new SchedulerView();
        }
    }
}