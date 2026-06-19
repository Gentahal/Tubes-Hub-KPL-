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
                CmbStartMonth.SelectedIndex = ProjectManager.StartMonth - 1;
                CmbStartMonth.IsEnabled = false;
                BtnInitProject.IsEnabled = false;
                BtnInitProject.Content = "✓ Proyek Aktif";
                
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
            if (ProjectManager.IsInitialized)
            {
                TxtStatus.Foreground = Brushes.Orange;
                TxtStatus.Text = "[Info] Proyek sudah diinisialisasi sebelumnya.";
                return;
            }

            int startMonth = CmbStartMonth.SelectedIndex + 1;

            ProjectManager.InitializeProject(startMonth);

            // Lock init controls
            CmbStartMonth.IsEnabled = false;
            BtnInitProject.IsEnabled = false;
            BtnInitProject.Content = "✓ Proyek Aktif";

            // Enable task input
            PnlInputTask.IsEnabled = true;
            PnlInputTask.Opacity = 1.0;

            TxtStatus.Foreground = Brushes.DarkGreen;
            TxtStatus.Text = $"[BERHASIL] Proyek diinisialisasi mulai bulan {ProjectManager.GetMonthName(1)}.";

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

            TimelinePanel.Items.Clear();

            var tasks = ProjectManager.Tasks;
            if (tasks.Count == 0)
            {
                TimelinePanel.Items.Add(new TextBlock
                {
                    Text = "Belum ada tugas yang direncanakan. Gunakan form di atas untuk menambah tugas.",
                    FontStyle = FontStyle.Italic,
                    Margin = new Thickness(0, 5, 0, 5),
                    Foreground = Brushes.DarkGray
                });
                return;
            }

            var groupedTasks = tasks.OrderBy(t => t.RelativeMonth).GroupBy(t => t.RelativeMonth);

            foreach (var group in groupedTasks)
            {
                int totalWeight = group.Sum(t => t.Weight);
                string monthName = ProjectManager.GetMonthName(group.Key).ToUpper();
                double fillPercent = (double)totalWeight / ProjectManager.MaxWeightPerMonth * 100;

                // Month header with capacity bar
                var headerPanel = new StackPanel { Spacing = 4, Margin = new Thickness(0, 10, 0, 4) };
                headerPanel.Children.Add(new TextBlock
                {
                    Text = $"{monthName} (Beban: {totalWeight}/{ProjectManager.MaxWeightPerMonth})",
                    FontWeight = FontWeight.Bold,
                    FontSize = 14,
                    Foreground = Brushes.Black
                });

                // Capacity progress bar
                var capacityBar = new ProgressBar
                {
                    Minimum = 0,
                    Maximum = 100,
                    Value = fillPercent,
                    Height = 6,
                    CornerRadius = new CornerRadius(3),
                    Foreground = fillPercent > 80 ? Brushes.OrangeRed : Brushes.DodgerBlue,
                    Background = Brush.Parse("#E8E8E8")
                };
                headerPanel.Children.Add(capacityBar);
                TimelinePanel.Items.Add(headerPanel);

                TimelinePanel.Items.Add(new Rectangle
                {
                    Height = 1,
                    Fill = Brushes.LightGray,
                    Margin = new Thickness(0, 2, 0, 4),
                    HorizontalAlignment = HorizontalAlignment.Stretch
                });

                // Task grid with header
                var grid = new Grid
                {
                    Margin = new Thickness(10, 2, 0, 2)
                };
                grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(100)));
                grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(200)));
                grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
                grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(70)));
                grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(70)));

                int row = 0;
                foreach (var task in group)
                {
                    grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

                    AddCell(grid, task.Category, row, 0, Brushes.Gray, FontWeight.Normal);
                    AddCell(grid, task.Title, row, 1, Brushes.Black, FontWeight.SemiBold);
                    AddCell(grid, task.Detail, row, 2, Brushes.DimGray, FontWeight.Normal);
                    AddCell(grid, $"{task.EstimatedDays}d", row, 3, Brushes.DarkBlue, FontWeight.Normal);
                    
                    // Bobot level with color coding
                    var bobotColor = task.BobotLevel == "Berat" ? Brushes.OrangeRed : Brushes.Green;
                    AddCell(grid, task.BobotLevel, row, 4, bobotColor, FontWeight.Bold);

                    row++;
                }

                TimelinePanel.Items.Add(grid);
            }
        }

        private void AddCell(Grid grid, string text, int row, int col, IBrush foreground, FontWeight weight)
        {
            var tb = new TextBlock
            {
                Text = text,
                Margin = new Thickness(2, 3, 2, 3),
                TextWrapping = TextWrapping.Wrap,
                Foreground = foreground,
                FontWeight = weight,
                FontSize = 13
            };
            Grid.SetRow(tb, row);
            Grid.SetColumn(tb, col);
            grid.Children.Add(tb);
        }
    }
}
