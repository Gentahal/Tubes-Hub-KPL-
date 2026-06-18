using Avalonia.Controls;
using Avalonia.Interactivity;
using tubes_hub.Tubes_Hub_KPL_;

namespace Tubes_Hub_WBSGUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnProgress_Click(object? sender, RoutedEventArgs e)
        {
            // TODO: buka window Modul Progress milik temanmu
        }

        private void BtnWBS_Click(object? sender, RoutedEventArgs e)
        {
            var wbsWindow = new WBSWindow();
            wbsWindow.Show();
        }

        private void BtnTeamInfo_Click(object? sender, RoutedEventArgs e)
        {
            // TODO: buka window Modul Team Info milik temanmu
        }

        private void BtnScheduler_Click(object? sender, RoutedEventArgs e)
        {
            // TODO: buka window Modul Scheduler milik temanmu
        }

        private void BtnFinalisasi_Click(object? sender, RoutedEventArgs e)
        {
            // TODO: buka window Modul Finalisasi milik temanmu
        }

        private void BtnExit_Click(object? sender, RoutedEventArgs e)
        {
            if (Avalonia.Application.Current?.ApplicationLifetime is
                Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime lifetime)
            {
                lifetime.Shutdown();
            }
        }
    }
}

