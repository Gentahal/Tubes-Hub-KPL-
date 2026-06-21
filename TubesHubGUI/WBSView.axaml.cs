using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using TubesHub;

namespace TubesHubGUI
{
    public partial class WBSView : UserControl
    {
        public WBSView()
        {
            InitializeComponent();
            
            // Sync with ProjectManager on load
            if (ProjectManager.IsInitialized)
            {
                // Project already initialized — show the selected month and enable inputs
                DatePickerStart.SelectedDate = ProjectManager.StartDate;
                BtnInitProject.Content = "Mulai Ulang Proyek Baru";
                
                PnlInputTask.IsEnabled = true;
                PnlInputTask.Opacity = 1.0;

                RefreshTimeline();
            }
            else
            {
                // Show welcome state
                TxtStatus.Text = "Pilih bulan mulai proyek dan klik 'Inisialisasi Proyek' untuk memulai.";
            }
        }

        private void BtnInitProject_Click(object? sender, RoutedEventArgs e)
        {
            // Removed the check that prevents re-initialization so user can input another project

            if (!DatePickerStart.SelectedDate.HasValue)
            {
                TxtStatus.Foreground = Brushes.Red;
                TxtStatus.Text = "[ERROR] Pilih tanggal mulai proyek terlebih dahulu.";
                return;
            }

            DateTime selectedDate = DatePickerStart.SelectedDate.Value;

            if (selectedDate.Date < DateTime.Today)
            {
                TxtStatus.Foreground = Brushes.Red;
                TxtStatus.Text = "[ERROR] Tanggal mulai tidak boleh di masa lampau.";
                return;
            }

            ProjectManager.InitializeProject(selectedDate);

            BtnInitProject.Content = "Mulai Ulang Proyek Baru";

            // Enable task input
            PnlInputTask.IsEnabled = true;
            PnlInputTask.Opacity = 1.0;

            TxtStatus.Foreground = Brushes.DarkGreen;
            TxtStatus.Text = $"[BERHASIL] Proyek diinisialisasi mulai tanggal {ProjectManager.StartDate:dd MMMM yyyy}.";

            RefreshTimeline();
        }

        private void BtnAddTask_Click(object? sender, RoutedEventArgs e)
        {
            if (!ProjectManager.IsInitialized)
            {
                TxtStatus.Foreground = Brushes.Red;
                TxtStatus.Text = "[ERROR] Inisialisasi proyek terlebih dahulu.";
                return;
            }

            if (CmbCategory.SelectedItem is not ComboBoxItem selectedItem)
            {
                TxtStatus.Foreground = Brushes.Red;
                TxtStatus.Text = "[ERROR] Pilih kategori tugas.";
                return;
            }

            string category = selectedItem.Content?.ToString() ?? "";
            string title = TxtTitle.Text?.Trim() ?? "";
            string desc = TxtDesc.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(title))
            {
                TxtStatus.Foreground = Brushes.Red;
                TxtStatus.Text = "[ERROR] Judul tugas tidak boleh kosong.";
                return;
            }

            try
            {
                ProjectManager.AddTaskWBS(category, title, desc);

                TxtStatus.Foreground = Brushes.DarkGreen;
                TxtStatus.Text = $"[BERHASIL] Tugas '{title}' ditambahkan ke kategori {category}.";

                TxtTitle.Text = "";
                TxtDesc.Text = "";
                CmbCategory.SelectedIndex = -1;

                RefreshTimeline();
            }
            catch (Exception ex)
            {
                TxtStatus.Foreground = Brushes.Red;
                TxtStatus.Text = $"[ERROR] {ex.Message}";
            }
        }

        private void RefreshTimeline()
        {
            if (!ProjectManager.IsInitialized) return;

            var tasks = ProjectManager.Tasks;
            
            // Format color string for binding evaluation if needed
            var formattedTasks = tasks.Select(t => new {
                t.Id,
                t.Category,
                t.Title,
                t.Detail,
                t.EstimatedDays,
                t.BobotLevel,
                BobotColor = t.BobotLevel == "Berat" ? "OrangeRed" : "Green"
            }).ToList();

            TaskListPanel.ItemsSource = formattedTasks;
        }

        private void BtnDeleteTask_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Guid taskId)
            {
                ProjectManager.RemoveTaskWBS(taskId);
                RefreshTimeline();
                TxtStatus.Foreground = Brushes.DarkGreen;
                TxtStatus.Text = "[BERHASIL] Tugas berhasil dihapus.";
            }
        }
    }
}
