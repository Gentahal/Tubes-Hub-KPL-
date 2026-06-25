using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System.Collections.Generic;
using System.Linq;
using TubesHub;

namespace TubesHubGUI
{
    public partial class TeamInfoView : UserControl
    {
        // Daftar Role yang ditawarkan di dropdown sebagai pilihan cepat.
        // "+ Role Baru..." memicu munculnya TextBox untuk ketik Role custom.
        private static readonly string[] RolePilihanCepat =
        {
            "WBS", "Progress", "Team Info", "Scheduler", "Finalisasi", "+ Role Baru..."
        };

        private const string OpsiRoleBaru = "+ Role Baru...";

        public TeamInfoView()
        {
            InitializeComponent();
            LoadTeamData();
        }

        private void LoadTeamData()
        {
            if (ProjectManager.Members.Count == 0)
            {
                ProjectManager.LoadTeamMembers();
            }

            PnlMembers.Children.Clear();

            if (ProjectManager.Members.Count == 0)
            {
                LblStatus.Text = "Gagal memuat data anggota. Pastikan file team.json tersedia di direktori proyek.";
                StatusBar.Background = Brush.Parse("#FFEBEE");
                LblStatus.Foreground = Brush.Parse("#C62828");
                return;
            }

            LblStatus.Text = $"Data {ProjectManager.Members.Count} anggota berhasil dimuat dari team.json (Runtime Config).";

            foreach (var member in ProjectManager.Members)
            {
                var memberTasks = ProjectManager.Tasks
                    .Where(t => t.AssignedTo == member.Nama)
                    .ToList();

                // Member card
                var card = new Border
                {
                    Background = Brushes.White,
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(18),
                    BoxShadow = BoxShadows.Parse("0 2 6 0 #18000000")
                };

                var cardContent = new StackPanel { Spacing = 8 };

                // Name and NIM row
                var headerGrid = new Grid();
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

                var nameBlock = new TextBlock
                {
                    Text = member.Nama,
                    FontSize = 16,
                    FontWeight = FontWeight.Bold,
                    Foreground = Brush.Parse("#1E1E1E")
                };
                Grid.SetColumn(nameBlock, 0);
                headerGrid.Children.Add(nameBlock);

                var roleBadge = new Border
                {
                    Background = GetRoleBadgeColor(member.Role),
                    CornerRadius = new CornerRadius(4),
                    Padding = new Thickness(10, 4),
                    Child = new TextBlock
                    {
                        Text = member.Role,
                        Foreground = Brushes.White,
                        FontSize = 12,
                        FontWeight = FontWeight.Bold
                    }
                };
                Grid.SetColumn(roleBadge, 1);
                headerGrid.Children.Add(roleBadge);

                cardContent.Children.Add(headerGrid);

                // NIM
                cardContent.Children.Add(new TextBlock
                {
                    Text = $"NIM: {member.Nim}",
                    FontSize = 13,
                    Foreground = Brush.Parse("#888888")
                });

                // Separator
                cardContent.Children.Add(new Border
                {
                    Height = 1,
                    Background = Brush.Parse("#EEEEEE"),
                    Margin = new Thickness(0, 4)
                });

                // Task assignments
                if (memberTasks.Count > 0)
                {
                    cardContent.Children.Add(new TextBlock
                    {
                        Text = $"Tugas yang diassign ({memberTasks.Count}):",
                        FontSize = 13,
                        FontWeight = FontWeight.SemiBold,
                        Foreground = Brush.Parse("#555555")
                    });

                    foreach (var task in memberTasks)
                    {
                        var statusColor = task.CurrentState switch
                        {
                            TaskState.Done => Brush.Parse("#28A745"),
                            TaskState.InProgress => Brush.Parse("#0078D7"),
                            _ => Brush.Parse("#888888")
                        };

                        var taskRow = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Margin = new Thickness(10, 2, 0, 2) };
                        taskRow.Children.Add(new TextBlock { Text = "•", Foreground = statusColor, FontWeight = FontWeight.Bold });
                        taskRow.Children.Add(new TextBlock
                        {
                            Text = $"{task.Title} [{task.CurrentState}] {task.Progress}%",
                            FontSize = 13,
                            Foreground = statusColor
                        });
                        cardContent.Children.Add(taskRow);
                    }
                }
                else
                {
                    cardContent.Children.Add(new TextBlock
                    {
                        Text = "Belum ada tugas yang diassign. Assign di modul Progress.",
                        FontSize = 13,
                        FontStyle = FontStyle.Italic,
                        Foreground = Brush.Parse("#AAAAAA")
                    });
                }

                // Separator sebelum bagian edit Role
                cardContent.Children.Add(new Border
                {
                    Height = 1,
                    Background = Brush.Parse("#EEEEEE"),
                    Margin = new Thickness(0, 4)
                });

                // --- Bagian Ganti Role ---
                cardContent.Children.Add(BuatPanelGantiRole(member));

                card.Child = cardContent;
                PnlMembers.Children.Add(card);
            }
        }

        private StackPanel BuatPanelGantiRole(TeamMember member)
        {
            var panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };

            var roleDropdown = new ComboBox
            {
                Width = 160,
                ItemsSource = RolePilihanCepat,
                SelectedItem = RolePilihanCepat.Contains(member.Role) ? member.Role : OpsiRoleBaru
            };

            var roleBaruTextBox = new TextBox
            {
                Width = 140,
                Watermark = "Nama Role baru",
                IsVisible = !RolePilihanCepat.Contains(member.Role)
            };

            if (!RolePilihanCepat.Contains(member.Role))
            {
                roleBaruTextBox.Text = member.Role;
            }

            roleDropdown.SelectionChanged += (_, _) =>
            {
                roleBaruTextBox.IsVisible = roleDropdown.SelectedItem as string == OpsiRoleBaru;
            };

            var statusText = new TextBlock
            {
                FontSize = 12,
                Foreground = Brush.Parse("#888888"),
                VerticalAlignment = VerticalAlignment.Center
            };

            var simpanButton = new Button
            {
                Content = "Simpan Role",
                Padding = new Thickness(12, 4)
            };

            simpanButton.Click += (_, _) =>
            {
                string roleTerpilih = roleDropdown.SelectedItem as string ?? "";
                string roleBaru = roleTerpilih == OpsiRoleBaru
                    ? roleBaruTextBox.Text?.Trim() ?? ""
                    : roleTerpilih;

                // --- Defensive check (DbC) ---
                if (string.IsNullOrWhiteSpace(roleBaru))
                {
                    statusText.Text = "Role tidak boleh kosong.";
                    statusText.Foreground = Brush.Parse("#C62828");
                    return;
                }

                if (roleBaru.Equals(member.Role, System.StringComparison.OrdinalIgnoreCase))
                {
                    statusText.Text = $"Role sudah '{roleBaru}'.";
                    statusText.Foreground = Brush.Parse("#888888");
                    return;
                }

                string roleLama = member.Role;
                member.Role = roleBaru;

                statusText.Text = $"Role diubah dari '{roleLama}' ke '{roleBaru}' (sesi ini saja).";
                statusText.Foreground = Brush.Parse("#2E7D32");

                // Refresh seluruh tampilan supaya badge Role ikut update
                LoadTeamData();
            };

            panel.Children.Add(roleDropdown);
            panel.Children.Add(roleBaruTextBox);
            panel.Children.Add(simpanButton);
            panel.Children.Add(statusText);

            return panel;
        }

        private static IBrush GetRoleBadgeColor(string role)
        {
            return role switch
            {
                "WBS" => Brush.Parse("#0078D7"),
                "Progress" => Brush.Parse("#E67E22"),
                "Team Info" => Brush.Parse("#8E44AD"),
                "Scheduler" => Brush.Parse("#27AE60"),
                "Finalisasi" => Brush.Parse("#C0392B"),
                _ => Brush.Parse("#555555")
            };
        }
    }
}