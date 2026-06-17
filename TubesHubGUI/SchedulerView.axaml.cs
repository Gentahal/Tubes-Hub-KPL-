using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic; // Mengaktifkan List untuk proses filtering

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
        }

        private void BtnCek_Click(object sender, RoutedEventArgs e)
        {
            // Validasi Input (Defensive UX)
            if (CmbHari.SelectedIndex == -1)
            {
                LblSaran.Text = "[ERROR] Silakan pilih hari terlebih dahulu.";
                PnlSaran.Background = Brushes.Crimson;
                return;
            }

            // Ekstrak data Table-Driven untuk hari terpilih (hanya sebagai backup / jika cuaca panas)
            int indeksHari = CmbHari.SelectedIndex;
            string lokasiHariIni = LokasiKumpul[indeksHari];

            LblLokasi.Text = "⏳ Memproses lokasi...";
            LblSuhu.Text = "⏳ Mengambil data cuaca...";
            PnlSaran.Background = Brushes.Gray;

            // Mengecek cuaca via API eksternal (Open-Meteo)
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

                            LblSuhu.Text = $"🌡️ Suhu saat ini: {suhu}°C";

                            // KONDISI 1: CUACA PANAS (> 30 C) -> Tampilkan jadwal spesifik hari itu
                            if (suhu > 30)
                            {
                                LblLokasi.Text = $"📍 Lokasi kumpul hari ini: {lokasiHariIni}";
                                LblSaran.Text = "Saran: Cuaca cukup panas, sebaiknya kumpul di ruangan ber-AC or via Discord.";
                                PnlSaran.Background = Brushes.Crimson; // Merah
                            }
                            // KONDISI 2: CUACA ADEM (<= 30 C) -> Output seragam untuk semua hari
                            else
                            {
                                LblSaran.Text = "Saran: Cuaca sangat mendukung untuk diskusi tatap muka!";
                                PnlSaran.Background = Brushes.ForestGreen; // Hijau

                                // Filter semua lokasi tatap muka dari array (Buang Discord dan Libur)
                                List<string> tempatTatapMuka = new List<string>();
                                foreach (string tempat in LokasiKumpul)
                                {
                                    if (tempat != "Discord (Online)" && tempat != "Libur" && !tempatTatapMuka.Contains(tempat))
                                    {
                                        tempatTatapMuka.Add(tempat);
                                    }
                                }

                                // Gabungkan list tempat menjadi string terpisah koma
                                string semuaTempatAman = string.Join(", ", tempatTatapMuka);

                                // Tampilkan output yang sama/seragam tanpa peduli hari apa yang dipilih
                                LblLokasi.Text = $"✨ Semua Tempat Tersedia (Tatap Muka):\n👉 {semuaTempatAman}";
                            }
                        }
                    }
                    else
                    {
                        LblSuhu.Text = "[API Error] Gagal mendapatkan data cuaca.";
                        LblLokasi.Text = $"📍 Lokasi kumpul utama: {lokasiHariIni}";
                        LblSaran.Text = "Saran: Tetap ikuti lokasi kumpul utama.";
                    }
                }
            }
            catch (Exception ex)
            {
                LblSuhu.Text = $"[API Error] Koneksi bermasalah: {ex.Message}";
                LblLokasi.Text = $"📍 Lokasi kumpul utama: {lokasiHariIni}";
                LblSaran.Text = "Saran: Mode Offline. Ikuti lokasi kumpul utama.";
            }
        }
    }
}