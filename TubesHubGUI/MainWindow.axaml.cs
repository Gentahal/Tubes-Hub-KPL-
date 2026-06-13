using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TubesHubGUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Saat aplikasi pertama dibuka, langsung tampilkan halaman Finalisasi milikmu sebagai uji coba
            MainContentArea.Content = new FinalisasiView();
        }

        // --- FUNGSI TOMBOL NAVIGASI ---

        private void NavFinalisasi_Click(object sender, RoutedEventArgs e)
        {
            // Memanggil file FinalisasiView.axaml milikmu
            MainContentArea.Content = new FinalisasiView();
        }

        private void NavWbs_Click(object sender, RoutedEventArgs e)
        {
            // Karena Ahmad belum membuat halamannya, kita beri teks sementara
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
            MainContentArea.Content = new TextBlock { Text = "Halaman Scheduler milik Khaydir sedang dikerjakan...", FontSize = 18, Margin = new Avalonia.Thickness(20) };
        }
    }
}