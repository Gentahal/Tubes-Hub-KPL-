using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Diagnostics;
using TubesHub.ModulProgress; 

namespace TubesHubGUI
{
    public partial class ProgressView : UserControl
    {
        // Tambah '?' biar warning CS8618 hilang
        private TaskItem? activeTask;

        public ProgressView()
        {
            InitializeComponent();
        }

        // 1. Aksi buat tugas baru (Menerapkan status awal ToDo)
        private void CreateTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = TaskNameInput.Text ?? "";
                if (string.IsNullOrWhiteSpace(name))
                {
                    OutputText.Text = "[DbC Error] Nama tugas tidak boleh kosong!";
                    return;
                }
                
                activeTask = new TaskItem(name);
                OutputText.Text = $"[Sukses] Tugas '{activeTask.Title}' dibuat. Status Awal: {activeTask.CurrentState}";
            }
            catch (Exception ex)
            {
                OutputText.Text = $"[Error] {ex.Message}";
            }
        }

        // 2. Aksi cek API Kalender Libur Nasional
        private async void CheckApi_Click(object sender, RoutedEventArgs e)
        {
            string inputDate = DateInput.Text ?? "";
            if (DateTime.TryParse(inputDate, out DateTime deadlineDate))
            {
                OutputText.Text = "Menghubungi Web API Nager.Date...";
                Stopwatch sw = Stopwatch.StartNew();

                try
                {
                    bool isLibur = await HolidayChecker.IsHolidayAsync(deadlineDate);
                    sw.Stop();

                    if (isLibur)
                    {
                        OutputText.Text = $"[Peringatan API] Tanggal {inputDate} adalah hari libur nasional!\nRespons API: {sw.ElapsedMilliseconds} ms";
                    }
                    else
                    {
                        OutputText.Text = $"[Aman] Tanggal {inputDate} bukan hari libur.\nRespons API: {sw.ElapsedMilliseconds} ms";
                    }
                }
                catch (Exception ex)
                {
                    OutputText.Text = $"[API Error] Gagal mengambil data: {ex.Message}";
                }
            }
            else
            {
                OutputText.Text = "[Error] Format tanggal salah! Gunakan YYYY-MM-DD.";
            }
        }

        // 3. Aksi Ubah Status (Validasi Automata)
        private void ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            if (activeTask == null)
            {
                OutputText.Text = "[Peringatan] Buat tugas terlebih dahulu!";
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

                // Gaskeun Panggil mesin automata dari backend
                activeTask.TransitionTo(targetState);
                OutputText.Text = $"[Automata Sukses] Status berhasil diubah menjadi: {activeTask.CurrentState}";
            }
            catch (Exception ex)
            {
                // Menangkap InvalidOperationException kalau statusnya melompat
                OutputText.Text = $"[Ditolak Automata] {ex.Message}";
            }
        }

        // 4. Aksi Update Progress (Validasi DbC) - INI YANG TADI BIKIN ERROR
        private void UpdateProgress_Click(object sender, RoutedEventArgs e)
        {
            if (activeTask == null)
            {
                OutputText.Text = "[Peringatan] Buat tugas terlebih dahulu!";
                return;
            }

            if (int.TryParse(ProgressInput.Text, out int persentase))
            {
                try
                {
                    activeTask.UpdateProgress(persentase);
                    OutputText.Text = $"[DbC Sukses] Progress tugas diperbarui menjadi {persentase}%.";
                }
                catch (Exception ex)
                {
                    OutputText.Text = $"[Ditolak DbC] {ex.Message}";
                }
            }
            else
            {
                OutputText.Text = "[Error] Harap masukkan angka yang valid (0-100).";
            }
        }
    }
}