using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TubesHubGUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // Pre-load team data from team.json (Runtime Config)
            TubesHub.ProjectManager.LoadTeamMembers();
            MainContentArea.Content = new WBSView();
        }

        private void SetActiveMenu(Button activeButton)
        {
            var buttons = new[] { BtnWbs, BtnProgress, BtnTeam, BtnScheduler, BtnFinalisasi };
            foreach (var btn in buttons)
            {
                if (btn == null) continue;
                if (btn == activeButton)
                {
                    btn.Background = Avalonia.Media.Brush.Parse("#0078D7");
                    btn.FontWeight = Avalonia.Media.FontWeight.Bold;
                }
                else
                {
                    btn.Background = Avalonia.Media.Brush.Parse("#3E3E42");
                    btn.FontWeight = Avalonia.Media.FontWeight.Normal;
                }
            }
        }

        private void NavFinalisasi_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new FinalisasiView();
            SetActiveMenu(BtnFinalisasi);
        }

        private void NavWbs_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new WBSView();
            SetActiveMenu(BtnWbs);
        }

        private void NavProgress_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new ProgressView();
            SetActiveMenu(BtnProgress);
        }

        private void NavTeam_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new TeamInfoView();
            SetActiveMenu(BtnTeam);
        }

        private void NavScheduler_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new SchedulerView();
            SetActiveMenu(BtnScheduler);
        }
    }
}