using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using TubesHub;

namespace TubesHubGUI
{
    public partial class SchedulerView : UserControl
    {
        private static readonly string[] LokasiKumpul = {
            "Laboratorium KPL", "Kantin Teknik", "Perpustakaan",
            "Discord (Online)", "Cafe", "Libur", "Libur"
        };

        public SchedulerView()
        {
            InitializeComponent();
            var cmb = this.FindControl<ComboBox>("CmbHari");
            if (cmb != null) cmb.SelectedIndex = 0;
            LoadPendingTasks();
        }

        private void LoadPendingTasks()
        {
            var pnlTugas = this.FindControl<StackPanel>("PnlTugasPending");
            if (pnlTugas == null) return;
            
            pnlTugas.Children.Clear();

            var pendingTasks = ProjectManager.Tasks.Where(t => t.CurrentState != TaskState.Done).ToList();
            if (pendingTasks.Count == 0)
            {
                pnlTugas.Children.Add(new TextBlock { Text = "Semua tugas sudah selesai! Tidak ada beban pikiran.", Foreground = Brushes.Green, FontStyle = FontStyle.Italic });
                return;
            }

            foreach (var task in pendingTasks)
            {
                string deadlineStr = task.DueDate.HasValue ? task.DueDate.Value.ToString("dd MMM yyyy") : "Belum diatur";
                string assignStr = string.IsNullOrEmpty(task.AssignedTo) ? "Belum diassign" : task.AssignedTo;
                
                var border = new Border
                {
                    Background = Brushes.White,
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Avalonia.Thickness(1),
                    CornerRadius = new Avalonia.CornerRadius(4),
                    Padding = new Avalonia.Thickness(10),
                    Margin = new Avalonia.Thickness(0, 0, 0, 10),
                    Child = new StackPanel
                    {
                        Spacing = 5,
                        Children = {
                            new TextBlock { Text = task.Title, FontWeight = FontWeight.Bold, Foreground = Brushes.Black },
                            new TextBlock { Text = $"PIC: {assignStr} | Deadline: {deadlineStr}", FontSize = 12, Foreground = Brushes.Gray },
                            new TextBlock { Text = $"Status: {task.CurrentState} | Progress: {task.Progress}%", FontSize = 12, Foreground = Brushes.DarkBlue }
                        }
                    }
                };
                pnlTugas.Children.Add(border);
            }
        }

        private void BtnCek_Click(object sender, RoutedEventArgs e)
        {
            var cmbHari = this.FindControl<ComboBox>("CmbHari");
            var lblLokasi = this.FindControl<TextBlock>("LblLokasi");
            var lblSuhu = this.FindControl<TextBlock>("LblSuhu");
            var lblSaran = this.FindControl<TextBlock>("LblSaran");
            var pnlSaran = this.FindControl<Border>("PnlSaran");

            if (cmbHari == null || cmbHari.SelectedIndex == -1)
            {
                if (lblSaran != null) lblSaran.Text = "[ERROR] Silakan pilih hari terlebih dahulu.";
                if (pnlSaran != null) pnlSaran.Background = Brushes.Crimson;
                return;
            }

            int indeksHari = cmbHari.SelectedIndex;
            string lokasiHariIni = LokasiKumpul[indeksHari];

            if (lblLokasi != null) lblLokasi.Text = "⏳ Memproses lokasi...";
            if (lblSuhu != null) lblSuhu.Text = "⏳ Mengambil data cuaca...";
            if (pnlSaran != null) pnlSaran.Background = Brushes.Gray;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = "https://api.open-meteo.com/v1/forecast?latitude=-6.97&longitude=107.63&current_weather=true";
                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = response.Content.ReadAsStringAsync().Result;
                        using (JsonDocument document = JsonDocument.Parse(jsonString))
                        {
                            JsonElement currentWeather = document.RootElement.GetProperty("current_weather");
                            double suhu = currentWeather.GetProperty("temperature").GetDouble();

                            if (lblSuhu != null) lblSuhu.Text = $"🌡️ Suhu saat ini: {suhu}°C.";

                            if (lokasiHariIni == "Libur")
                            {
                                if (lblLokasi != null) lblLokasi.Text = "✨ Jadwal Hari Ini:\n👉 Libur (Selamat Berakhir Pekan! 🎉)";
                                if (lblSaran != null) lblSaran.Text = "Saran: Hari ini jadwalnya libur, silakan kerjakan tugas mandiri atau istirahat.";
                                if (pnlSaran != null) pnlSaran.Background = Brushes.DarkGreen;
                            }
                            else if (suhu > 30)
                            {
                                if (lblLokasi != null) lblLokasi.Text = $"📍 Lokasi kumpul hari ini: {lokasiHariIni}";
                                if (lblSaran != null) lblSaran.Text = "Saran: Cuaca panas, sebaiknya kumpul di ruangan ber-AC atau Discord.";
                                if (pnlSaran != null) pnlSaran.Background = Brushes.Crimson;
                            }
                            else
                            {
                                if (lblSaran != null) lblSaran.Text = "Saran: Cuaca mendukung untuk diskusi kelompok tatap muka!";
                                if (pnlSaran != null) pnlSaran.Background = Brushes.DarkGreen;

                                List<string> tempatTatapMuka = new List<string>();
                                foreach (string tempat in LokasiKumpul)
                                {
                                    if (tempat != "Discord (Online)" && tempat != "Libur" && !tempatTatapMuka.Contains(tempat))
                                    {
                                        tempatTatapMuka.Add(tempat);
                                    }
                                }

                                string semuaTempatAman = string.Join(", ", tempatTatapMuka);
                                if (lblLokasi != null) lblLokasi.Text = $"✨ Semua Tempat Tersedia (Tatap Muka):\n👉 {semuaTempatAman}";
                            }
                        }
                    }
                    else
                    {
                        if (lblSuhu != null) lblSuhu.Text = "[API Error] Gagal mendapatkan data cuaca.";
                        if (lblLokasi != null) lblLokasi.Text = $"📍 Lokasi kumpul utama: {lokasiHariIni}";
                        if (lblSaran != null) lblSaran.Text = "Saran: Tetap ikuti lokasi kumpul utama.";
                    }
                }
            }
            catch (Exception ex)
            {
                if (lblSuhu != null) lblSuhu.Text = $"[API Error] Koneksi bermasalah: {ex.Message}";
                if (lblLokasi != null) lblLokasi.Text = $"📍 Lokasi kumpul utama: {lokasiHariIni}";
                if (lblSaran != null) lblSaran.Text = "Saran: Mode Offline. Ikuti lokasi kumpul utama.";
            }
        }
    }
}