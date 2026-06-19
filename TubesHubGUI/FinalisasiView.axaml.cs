using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Linq;
using TubesHub;

namespace TubesHubGUI
{
    public partial class FinalisasiView : UserControl
    {
        public FinalisasiView()
        {
            InitializeComponent();
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            var tasks = ProjectManager.Tasks;
            int totalTasks = tasks.Count;
            int doneTasks = tasks.Count(t => t.CurrentState == TaskState.Done);
            
            double averageProgress = 0;
            if (totalTasks > 0)
            {
                averageProgress = tasks.Average(t => t.Progress);
            }

            TxtTotalTasks.Text = totalTasks.ToString();
            TxtTasksDone.Text = doneTasks.ToString();
            
            ProjectProgressBar.Value = averageProgress;
            TxtProgressPercentage.Text = $"{Math.Round(averageProgress, 1)}% Selesai";

            UpdateDocStatusUI();
        }

        private void UpdateDocStatusUI()
        {
            TxtDocStatusInfo.Text = $"Status saat ini: {ProjectManager.DocAutomata.CurrentState}";
            CmbDocStatus.SelectedIndex = (int)ProjectManager.DocAutomata.CurrentState;
        }

        private void OnUpdateDocStatusClicked(object sender, RoutedEventArgs e)
        {
            if (CmbDocStatus.SelectedIndex == -1) return;

            DocumentState targetState = (DocumentState)CmbDocStatus.SelectedIndex;

            try
            {
                ProjectManager.DocAutomata.TransitionTo(targetState);
                UpdateDocStatusUI();
                PesanTextBlock.Foreground = Brushes.Green;
                PesanTextBlock.Text = $"[BERHASIL] Status dokumen diperbarui menjadi {targetState}.";
            }
            catch (Exception ex)
            {
                // Revert combobox to current state
                CmbDocStatus.SelectedIndex = (int)ProjectManager.DocAutomata.CurrentState;
                PesanTextBlock.Foreground = Brushes.Red;
                PesanTextBlock.Text = ex.Message;
            }
        }

        private void OnGenerateClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                ProjectManager.SaveReport();
                PesanTextBlock.Foreground = Brushes.Green;
                PesanTextBlock.Text = "[BERHASIL] Laporan akhir telah diekspor ke file 'laporan_akhir.json' di root direktori.";
            }
            catch (Exception ex)
            {
                PesanTextBlock.Foreground = Brushes.Red;
                PesanTextBlock.Text = $"[GAGAL] {ex.Message}";
            }
        }
    }
}