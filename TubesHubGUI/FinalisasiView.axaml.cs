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

            var doneTaskList = tasks.Where(t => t.CurrentState == TaskState.Done).ToList();
            CmbTaskFinalisasi.ItemsSource = doneTaskList;

            if (doneTaskList.Count == 0)
            {
                TxtDocStatusInfo.Text = "Belum ada tugas berstatus Done untuk didokumentasikan.";
                CmbDocStatus.IsEnabled = false;
            }
            else
            {
                CmbDocStatus.IsEnabled = true;
                if (CmbTaskFinalisasi.SelectedIndex == -1)
                {
                    TxtDocStatusInfo.Text = "Pilih tugas dari dropdown di atas.";
                    CmbDocStatus.SelectedIndex = -1;
                }
            }
        }

        private void CmbTaskFinalisasi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbTaskFinalisasi.SelectedItem is UnifiedTask selectedTask)
            {
                TxtDocStatusInfo.Text = $"Status saat ini: {selectedTask.DocState}";
                CmbDocStatus.SelectedIndex = (int)selectedTask.DocState;
            }
        }

        private void OnUpdateDocStatusClicked(object sender, RoutedEventArgs e)
        {
            if (CmbTaskFinalisasi.SelectedItem is not UnifiedTask selectedTask)
            {
                PesanTextBlock.Foreground = Brushes.Red;
                PesanTextBlock.Text = "[ERROR] Pilih tugas terlebih dahulu.";
                return;
            }

            if (CmbDocStatus.SelectedIndex == -1) return;

            DocumentState targetState = (DocumentState)CmbDocStatus.SelectedIndex;

            try
            {
                // To keep utilizing DocumentAutomata class, we can temporarily set its state, then transition
                // Alternatively we can use a fresh one to test the transition
                var automata = new DocumentAutomata();
                // We use a trick: transition one by one from Draft to reach current state, or just validate manually
                // The user requested to not be rigid: "jika user ingin sudah siap kumpul maka bisa di inputkan siap kumpul dari draft, tidak perlu revisi terlebih dahulu."
                if (selectedTask.DocState == targetState) return;

                // Set state directly without strict automata restriction for better UX
                selectedTask.DocState = targetState;
                TxtDocStatusInfo.Text = $"Status saat ini: {selectedTask.DocState}";
                PesanTextBlock.Foreground = Brushes.Green;
                PesanTextBlock.Text = $"[BERHASIL] Status dokumen untuk '{selectedTask.Title}' diperbarui menjadi {targetState}.";
            }
            catch (Exception ex)
            {
                // Revert combobox to current state
                CmbDocStatus.SelectedIndex = (int)selectedTask.DocState;
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