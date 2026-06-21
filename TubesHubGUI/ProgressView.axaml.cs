using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Linq;
using TubesHub;

namespace TubesHubGUI
{
    public partial class ProgressView : UserControl
    {
        private UnifiedTask? activeTask;

        public ProgressView()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            CmbTask.ItemsSource = null;
            CmbTask.ItemsSource = ProjectManager.Tasks;

            if (ProjectManager.Members.Count == 0)
            {
                ProjectManager.LoadTeamMembers();
            }
            CmbMembers.ItemsSource = null;
            CmbMembers.ItemsSource = ProjectManager.Members;

            // Show helpful message if no tasks or project not initialized
            if (!ProjectManager.IsInitialized)
            {
                LogToTerminal("[Info] Proyek belum diinisialisasi. Buka modul WBS terlebih dahulu untuk memulai.");
            }
            else if (ProjectManager.Tasks.Count == 0)
            {
                LogToTerminal("[Info] Belum ada tugas. Tambahkan tugas di modul WBS terlebih dahulu.");
            }
            else
            {
                LogToTerminal($"[Info] {ProjectManager.Tasks.Count} tugas ditemukan. Pilih tugas dari dropdown untuk mulai mengelola.");
            }
        }

        private void CmbTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbTask.SelectedItem is UnifiedTask task)
            {
                activeTask = task;
                
                // Update UI fields
                if (!string.IsNullOrEmpty(task.AssignedTo))
                {
                    var member = ProjectManager.Members.FirstOrDefault(m => m.Nama == task.AssignedTo);
                    CmbMembers.SelectedItem = member;
                }
                else
                {
                    CmbMembers.SelectedIndex = -1;
                }

                TaskDatePicker.SelectedDate = task.DueDate;
                
                StatusCombo.SelectedIndex = task.CurrentState switch
                {
                    TaskState.ToDo => 0,
                    TaskState.InProgress => 1,
                    TaskState.Done => 2,
                    _ => 0
                };

                ProgressInput.Text = task.Progress.ToString();
                LogToTerminal($"[Info] Memilih tugas: '{task.Title}' [{task.CurrentState}] Progress: {task.Progress}%");
            }
        }

        private void SaveProgress_Click(object sender, RoutedEventArgs e)
        {
            if (activeTask == null)
            {
                LogToTerminal("[Peringatan] Pilih tugas terlebih dahulu dari dropdown!");
                return;
            }

            try
            {
                // Set Assignee
                if (CmbMembers.SelectedItem is TeamMember member)
                {
                    activeTask.AssignedTo = member.Nama;
                }

                // Set Date
                if (TaskDatePicker.SelectedDate.HasValue)
                {
                    activeTask.DueDate = TaskDatePicker.SelectedDate.Value;
                }

                // Set Progress first (so we can check before status transition)
                int persentase = 0;
                if (int.TryParse(ProgressInput.Text, out persentase))
                {
                    TaskState targetState = StatusCombo.SelectedIndex switch
                    {
                        0 => TaskState.ToDo,
                        1 => TaskState.InProgress,
                        2 => TaskState.Done,
                        _ => TaskState.ToDo
                    };

                    // First update the state requested by the user manually, if different from current
                    if (activeTask.CurrentState != targetState)
                    {
                        activeTask.TransitionTo(targetState);
                    }

                    // Then update progress. Note that UpdateProgress will auto-adjust state if needed
                    // For example, if user sets Progress = 100, it automatically becomes Done.
                    activeTask.UpdateProgress(persentase);

                    // Sync UI back with actual task state
                    StatusCombo.SelectedIndex = activeTask.CurrentState switch
                    {
                        TaskState.ToDo => 0,
                        TaskState.InProgress => 1,
                        TaskState.Done => 2,
                        _ => 0
                    };
                    ProgressInput.Text = activeTask.Progress.ToString();
                }
                else
                {
                    LogToTerminal("[DbC Error] Progress harus berupa angka valid (0-100). Defensive Programming menolak input ini.");
                    return;
                }

                string assignedInfo = string.IsNullOrEmpty(activeTask.AssignedTo) ? "Belum diassign" : activeTask.AssignedTo;
                LogToTerminal($"[Sukses] Tugas '{activeTask.Title}' diperbarui → Status: {activeTask.CurrentState}, Progress: {activeTask.Progress}%, PIC: {assignedInfo}");
            }
            catch (Exception ex)
            {
                LogToTerminal($"[Ditolak - KPL Constraints] {ex.Message}");
                // Revert UI to actual state
                StatusCombo.SelectedIndex = activeTask.CurrentState switch
                {
                    TaskState.ToDo => 0,
                    TaskState.InProgress => 1,
                    TaskState.Done => 2,
                    _ => 0
                };
                ProgressInput.Text = activeTask.Progress.ToString();
            }
        }

        private void ViewHistory_Click(object sender, RoutedEventArgs e)
        {
            var tasks = ProjectManager.Tasks;
            if (tasks.Count == 0)
            {
                LogToTerminal("[-] Belum ada tugas di dalam WBS.");
                return;
            }

            LogToTerminal("=== REKAP TUGAS PROYEK ===");
            int aktif = 0;
            int selesai = 0;

            foreach (var task in tasks)
            {
                string status = task.CurrentState.ToString();
                if (task.CurrentState == TaskState.Done) selesai++;
                else aktif++;

                string assignStr = string.IsNullOrEmpty(task.AssignedTo) ? "Belum diassign" : task.AssignedTo;
                LogToTerminal($"- {task.Title} | PIC: {assignStr} | Status: {status} | Progress: {task.Progress}%");
            }
            
            double avgProgress = tasks.Average(t => t.Progress);
            LogToTerminal($"[Rekap] Total: {tasks.Count} | Aktif: {aktif} | Selesai: {selesai} | Rata-rata Progress: {Math.Round(avgProgress, 1)}%");
            LogToTerminal("============================");
        }

        private void LogToTerminal(string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            string newLog = $"[{time}] {message}\n";

            if (OutputText.Text != null && OutputText.Text.Contains("> Sistem siap."))
            {
                OutputText.Text = newLog;
            }
            else
            {
                OutputText.Text += newLog;
            }
        }
    }
}