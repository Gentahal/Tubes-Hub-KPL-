using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic; // Ditambahkan untuk menggunakan List
using TubesHub.ModulProgress; 

namespace TubesHubGUI
{
    public partial class ProgressView : UserControl
    {
        private TaskItem? activeTask;
        
        // List baru untuk menyimpan history semua tugas yang dibuat
        private List<TaskItem> taskHistory = new List<TaskItem>();

        public ProgressView()
        {
            InitializeComponent();
        }

        // Fungsi Helper buat bikin log terminal numpuk & ada jamnya
        private void LogToTerminal(string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            string newLog = $"[{time}] {message}\n";

            // Kalau ini log pertama, timpa teks default "Sistem siap..."
            if (OutputText.Text != null && OutputText.Text.Contains("> Sistem siap."))
            {
                OutputText.Text = newLog;
            }
            else
            {
                // Kalau udah ada isinya, tambahin ke baris bawahnya
                OutputText.Text += newLog;
            }
        }

        // 1. Aksi buat tugas baru
        private void CreateTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = TaskNameInput.Text ?? "";
                if (string.IsNullOrWhiteSpace(name))
                {
                    LogToTerminal("[DbC Error] Nama tugas tidak boleh kosong!");
                    return;
                }
                
                activeTask = new TaskItem(name);
                taskHistory.Add(activeTask); // Masukkan tugas baru ke dalam history
                
                LogToTerminal($"[Sukses] Tugas '{activeTask.Title}' dibuat. Status Awal: {activeTask.CurrentState}");
            }
            catch (Exception ex)
            {
                LogToTerminal($"[Error] {ex.Message}");
            }
        }

        // 2. Aksi Set Tanggal Pengerjaan
        private void SetDate_Click(object sender, RoutedEventArgs e)
        {
            if (activeTask == null)
            {
                LogToTerminal("[Peringatan] Buat tugas terlebih dahulu!");
                return;
            }

            if (TaskDatePicker.SelectedDate.HasValue)
            {
                DateTime selectedDate = TaskDatePicker.SelectedDate.Value;
                LogToTerminal($"[Sukses] Tanggal pengerjaan '{activeTask.Title}' diatur ke: {selectedDate.ToString("dd MMM yyyy")}");
            }
            else
            {
                LogToTerminal("[Peringatan] Harap pilih tanggal pengerjaan terlebih dahulu!");
            }
        }

        // 3. Aksi Ubah Status (Validasi Automata)
        private void ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            if (activeTask == null)
            {
                LogToTerminal("[Peringatan] Buat tugas terlebih dahulu!");
                return;
            }

            try
            {
                TaskState targetState = StatusCombo.SelectedIndex switch
                {
                    0 => TaskState.ToDo,
                    1 => TaskState.InProgress,
                    2 => TaskState.Done,
                    _ => TaskState.ToDo
                };

                activeTask.TransitionTo(targetState);
                LogToTerminal($"[Automata Sukses] Status berhasil diubah menjadi: {activeTask.CurrentState}");
            }
            catch (Exception ex)
            {
                LogToTerminal($"[Ditolak Automata] {ex.Message}");
            }
        }

        // 4. Aksi Update Progress (Validasi DbC)
        private void UpdateProgress_Click(object sender, RoutedEventArgs e)
        {
            if (activeTask == null)
            {
                LogToTerminal("[Peringatan] Buat tugas terlebih dahulu!");
                return;
            }

            if (int.TryParse(ProgressInput.Text, out int persentase))
            {
                try
                {
                    activeTask.UpdateProgress(persentase);
                    LogToTerminal($"[DbC Sukses] Progress tugas diperbarui menjadi {persentase}%.");
                }
                catch (Exception ex)
                {
                    LogToTerminal($"[Ditolak DbC] {ex.Message}");
                }
            }
            else
            {
                LogToTerminal("[Error] Harap masukkan angka yang valid (0-100).");
            }
        }

        // 5. Aksi Lihat History Tugas
        private void ViewHistory_Click(object sender, RoutedEventArgs e)
        {
            if (taskHistory.Count == 0)
            {
                LogToTerminal("[-] History kosong. Belum ada tugas yang dibuat.");
                return;
            }

            LogToTerminal("=== HISTORY & LIST TUGAS ===");
            int aktif = 0;
            int selesai = 0;

            foreach (var task in taskHistory)
            {
                string status = task.CurrentState.ToString();
                if (status == "Done") selesai++;
                else aktif++;

                LogToTerminal($"- {task.Title} | Status: {status} | Progress: {task.Progress}%");
            }
            
            LogToTerminal($"[Rekap] Total Tugas: {taskHistory.Count} | Aktif: {aktif} | Selesai: {selesai}");
            LogToTerminal("============================");
        }
    }
}