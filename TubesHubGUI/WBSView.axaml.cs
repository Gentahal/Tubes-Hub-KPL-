using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using tubes_hub.Tubes_Hub_KPL_;

namespace TubesHubGUI
{
    public partial class WBSView : UserControl
    {
        private WBSModule? _wbs;

        public WBSView()
        {
            InitializeComponent();
        }

        // 1. Inisialisasi proyek -> set bulan mulai, baru bisa mulai tambah tugas
        private void BtnInitProject_Click(object? sender, RoutedEventArgs e)
        {
            string input = TxtStartMonth.Text?.Trim() ?? "";

            if (!int.TryParse(input, out int startMonth) || startMonth < 1 || startMonth > 12)
            {
                TxtStatus.Foreground = Brushes.Red;
                TxtStatus.Text = "[ERROR] Masukkan bulan mulai proyek 1-12.";
                return;
            }

            _wbs = new WBSModule(startMonth);

            CmbCategory.IsEnabled = true;
            TxtTitle.IsEnabled = true;
            TxtDesc.IsEnabled = true;
            BtnAddTask.IsEnabled = true;
            TxtStartMonth.IsEnabled = false;
            BtnInitProject.IsEnabled = false;

            TxtStatus.Foreground = Brushes.DarkGreen;
            TxtStatus.Text = $"[BERHASIL] Proyek diinisialisasi mulai bulan ke-{startMonth}.";

            RefreshTimeline();
        }

        // 2. Tambah tugas baru ke WBS
        private void BtnAddTask_Click(object? sender, RoutedEventArgs e)
        {
            if (_wbs == null)
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
                _wbs.AddTask(category, title, desc);

                TxtStatus.Foreground = Brushes.DarkGreen;
                TxtStatus.Text = $"[BERHASIL] Tugas '{title}' ditambahkan.";

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

        // Render ulang timeline berdasarkan data WBSModule
        private void RefreshTimeline()
        {
            if (_wbs == null) return;

            TimelinePanel.Items.Clear();

            var groupedTasks = _wbs.GetGroupedTasks();

            if (groupedTasks.Count == 0)
            {
                TimelinePanel.Items.Add(new TextBlock
                {
                    Text = "Belum ada tugas yang direncanakan.",
                    FontStyle = FontStyle.Italic,
                    Margin = new Thickness(0, 5, 0, 5)
                });
                return;
            }

            foreach (var group in groupedTasks)
            {
                var header = new TextBlock
                {
                    Text = $"{group.MonthName} (Beban: {group.TotalWeight}/{_wbs.MaxWeightPerMonth})",
                    FontWeight = FontWeight.Bold,
                    FontSize = 14,
                    Margin = new Thickness(0, 10, 0, 2)
                };
                TimelinePanel.Items.Add(header);

                TimelinePanel.Items.Add(new Rectangle
                {
                    Height = 1,
                    Fill = Brushes.LightGray,
                    Margin = new Thickness(0, 2, 0, 4),
                    HorizontalAlignment = HorizontalAlignment.Stretch
                });

                var grid = new Grid
                {
                    Margin = new Thickness(10, 2, 0, 2)
                };
                grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(110)));
                grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(180)));
                grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
                grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(80)));

                int row = 0;
                foreach (var task in group.Tasks)
                {
                    grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

                    AddCell(grid, task.Category, row, 0);
                    AddCell(grid, task.Title, row, 1);
                    AddCell(grid, task.Detail, row, 2);
                    AddCell(grid, $"{task.EstimatedDays} Hari", row, 3);

                    row++;
                }

                TimelinePanel.Items.Add(grid);
            }
        }

        private void AddCell(Grid grid, string text, int row, int col)
        {
            var tb = new TextBlock
            {
                Text = text,
                Margin = new Thickness(2),
                TextWrapping = TextWrapping.Wrap
            };
            Grid.SetRow(tb, row);
            Grid.SetColumn(tb, col);
            grid.Children.Add(tb);
        }
    }
}
