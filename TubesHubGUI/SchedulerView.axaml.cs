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
        // Data jadwal kumpul (Table-Driven) Senin - Minggu
        private static readonly string[] LokasiKumpul = {
            "Laboratorium KPL", "Kantin Teknik", "Perpustakaan",
            "Discord (Online)", "Cafe", "Libur", "Libur"
        };

        public SchedulerView()
        {
            InitializeComponent();

            // Set default pilihan awal ke hari Senin
            var cmb = this.FindControl<ComboBox>("CmbHari");
            if (cmb != null) cmb.SelectedIndex = 0;
        }

        private void BtnCek_Click(object sender, RoutedEventArgs e)
        {
            //  elemen GUI dari file axaml
            var cmbHari = this.FindControl<ComboBox>("CmbHari");
            var lblLokasi = this.FindControl<TextBlock>("LblLokasi");
            var lblSuhu = this.FindControl<TextBlock>("LblSuhu");
            var lblSaran = this.FindControl<TextBlock>("LblSaran");
            var pnlSaran = this.FindControl<Border>("PnlSaran");

            // Validasi input kalau belum milih hari
            if (cmbHari == null || cmbHari.SelectedIndex == -1)
            {
                if (lblSaran != null) lblSaran.Text = "[ERROR] Silakan pilih hari terlebih dahulu.";
                return;
            }

            // Ambil jadwal lokasi berdasarkan indeks hari yang dipilih
            int indeksHari = cmbHari.SelectedIndex;
            string lokasiHariIni = LokasiKumpul[indeksHari];

            // Tampilan loading awal saat tombol diklik
            if (lblLokasi != null) lblLokasi.Text = "⏳ Memproses lokasi...";
            if (lblSuhu != null) lblSuhu.Text = "⏳ Mengambil data cuaca...";
            if (pnlSaran != null) pnlSaran.Background = Brushes.Gray;

            // Request data cuaca ke Open-Meteo API
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

                            // Cek apakah hari yang dipilih  jadwalnya libur
                            if (lokasiHariIni == "Libur")
                            {
                                if (lblLokasi != null) lblLokasi.Text = "✨ Jadwal Hari Ini:\n👉 Libur (Selamat Berakhir Pekan! 🎉)";
                                if (lblSaran != null) lblSaran.Text = "Saran: Hari ini jadwalnya libur, tidak ada agenda diskusi kelompok.";
                                if (pnlSaran != null) pnlSaran.Background = Brushes.DarkGreen;
                            }
                            // Kondisi kalau cuaca panas
                            else if (suhu > 30)
                            {
                                if (lblLokasi != null) lblLokasi.Text = $"📍 Lokasi kumpul hari ini: {lokasiHariIni}";
                                if (lblSaran != null) lblSaran.Text = "Saran: Cuaca cukup panas, sebaiknya kumpul di ruangan ber-AC atau via Discord.";
                                if (pnlSaran != null) pnlSaran.Background = Brushes.Crimson;
                            }
                            // Kondisi kalau cuaca sejuk 
                            else
                            {
                                if (lblSaran != null) lblSaran.Text = "Saran: Cuaca sangat mendukung untuk diskusi tatap muka!";
                                if (pnlSaran != null) pnlSaran.Background = Brushes.DarkGreen;

                                // Filter array untuk buang opsi online dan hari libur
                                List<string> tempatTatapMuka = new List<string>();
                                foreach (string tempat in LokasiKumpul)
                                {
                                    if (tempat != "Discord (Online)" && tempat != "Libur" && !tempatTatapMuka.Contains(tempat))
                                    {
                                        tempatTatapMuka.Add(tempat);
                                    }
                                }

                                // Menggabunngkan list lokasi jadi satu baris string teks
                                string semuaTempatAman = string.Join(", ", tempatTatapMuka);

                                if (lblLokasi != null)
                                {
                                    lblLokasi.Text = $" Semua Tempat Tersedia (Tatap Muka):\n {semuaTempatAman}";
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
                // Fallback jika laptop tidak ada koneksi internet / API down
                if (lblSuhu != null) lblSuhu.Text = $"[API Error] Koneksi bermasalah: {ex.Message}";
                if (lblLokasi != null) lblLokasi.Text = $"📍 Lokasi kumpul utama: {lokasiHariIni}";
                if (lblSaran != null) lblSaran.Text = "Saran: Mode Offline. Ikuti lokasi kumpul utama.";
                if (pnlSaran != null) pnlSaran.Background = Brushes.Gray;
            }
        }
    }
}