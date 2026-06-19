using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;

namespace TubesHubGUI
{
    public partial class SchedulerView : UserControl
    {
        // Jadwal kumpul kelompok
        private static readonly string[] LokasiKumpul = {
            "Laboratorium KPL", "Kantin Teknik", "Perpustakaan",
            "Discord (Online)", "Cafe", "Libur", "Libur"
        };

        public SchedulerView()
        {
            InitializeComponent();

            // Default pilih hari Senin
            var cmb = this.FindControl<ComboBox>("CmbHari");
            if (cmb != null) cmb.SelectedIndex = 0;
        }

        private void BtnCek_Click(object sender, RoutedEventArgs e)
        {
            // Ambil elemen dari file axaml
            var cmbHari = this.FindControl<ComboBox>("CmbHari");
            var lblLokasi = this.FindControl<TextBlock>("LblLokasi");
            var lblSuhu = this.FindControl<TextBlock>("LblSuhu");
            var lblSaran = this.FindControl<TextBlock>("LblSaran");
            var pnlSaran = this.FindControl<Border>("PnlSaran");

            // Cek kalau belum pilih hari
            if (cmbHari == null || cmbHari.SelectedIndex == -1)
            {
                if (lblSaran != null) lblSaran.Text = "[ERROR] Silakan pilih hari terlebih dahulu.";
                if (pnlSaran != null) pnlSaran.Background = Brushes.Crimson;
                return;
            }

            // Ambil data berdasarkan indeks hari
            int indeksHari = cmbHari.SelectedIndex;
            string lokasiHariIni = LokasiKumpul[indeksHari];

            // Efek loading awal
            if (lblLokasi != null) lblLokasi.Text = "⏳ Memproses lokasi...";
            if (lblSuhu != null) lblSuhu.Text = "⏳ Mengambil data cuaca...";
            if (pnlSaran != null) pnlSaran.Background = Brushes.Gray;

            // Ambil data cuaca dari API
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

                            // Cek jika hari libur (Sabtu/Minggu)
                            if (lokasiHariIni == "Libur")
                            {
                                if (lblLokasi != null) lblLokasi.Text = "✨ Jadwal Hari Ini:\n👉 Libur (Selamat Berakhir Pekan! 🎉)";
                                if (lblSaran != null) lblSaran.Text = "Saran: Hari ini jadwalnya libur, tidak ada agenda diskusi kelompok.";
                                if (pnlSaran != null) pnlSaran.Background = Brushes.DarkGreen;
                            }
                            // Cek jika cuaca panas
                            else if (suhu > 30)
                            {
                                if (lblLokasi != null) lblLokasi.Text = $"📍 Lokasi kumpul hari ini: {lokasiHariIni}";
                                if (lblSaran != null) lblSaran.Text = "Saran: Cuaca cukup panas, sebaiknya kumpul di ruangan ber-AC atau via Discord.";
                                if (pnlSaran != null) pnlSaran.Background = Brushes.Crimson;
                            }
                            // Cek jika cuaca adem
                            else
                            {
                                if (lblSaran != null) lblSaran.Text = "Saran: Cuaca sangat mendukung untuk diskusi tatap muka!";
                                if (pnlSaran != null) pnlSaran.Background = Brushes.DarkGreen;

                                // Filter tempat tatap muka saja
                                List<string> tempatTatapMuka = new List<string>();
                                foreach (string tempat in LokasiKumpul)
                                {
                                    if (tempat != "Discord (Online)" && tempat != "Libur" && !tempatTatapMuka.Contains(tempat))
                                    {
                                        tempatTatapMuka.Add(tempat);
                                    }
                                }

                                // Gabungkan list menjadi string teks
                                string semuaTempatAman = string.Join(", ", tempatTatapMuka);

                                if (lblLokasi != null)
                                {
                                    lblLokasi.Text = $"✨ Semua Tempat Tersedia (Tatap Muka):\n👉 {semuaTempatAman}";
                                }
                            }
                        }
                    }
                    else
                    {
                        if (lblSuhu != null) lblSuhu.Text = "[API Error] Gagal mendapatkan data cuaca.";
                        if (lblLokasi != null) lblLokasi.Text = $"📍 Lokasi kumpul utama: {lokasiHariIni}";
                        if (lblSaran != null) lblSaran.Text = "Saran: Tetap ikuti lokasi kumpul utama.";
                        if (pnlSaran != null) pnlSaran.Background = Brushes.Gray;
                    }
                }
            }
            catch (Exception ex)
            {
                // Fallback kalau offline / API gangguan
                if (lblSuhu != null) lblSuhu.Text = $"[API Error] Koneksi bermasalah: {ex.Message}";
                if (lblLokasi != null) lblLokasi.Text = $"📍 Lokasi kumpul utama: {lokasiHariIni}";
                if (lblSaran != null) lblSaran.Text = "Saran: Mode Offline. Ikuti lokasi kumpul utama.";
                if (pnlSaran != null) pnlSaran.Background = Brushes.Gray;
            }
        }
    }
}