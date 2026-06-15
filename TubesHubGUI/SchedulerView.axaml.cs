using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Net.Http;
using System.Text.Json;

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

            // Ekstrak data menggunakan Table-Driven
            int indeksHari = CmbHari.SelectedIndex;
            string lokasi = LokasiKumpul[indeksHari];

            LblLokasi.Text = $"📍 Lokasi kumpul: {lokasi}";
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

                            // Logika Keputusan
                            if (suhu > 30)
                            {
                                LblSaran.Text = "Saran: Cuaca cukup panas, sebaiknya kumpul di ruangan ber-AC atau via Discord.";
                                PnlSaran.Background = Brushes.Crimson; // Merah
                            }
                            else
                            {
                                LblSaran.Text = "Saran: Cuaca sangat mendukung untuk diskusi tatap muka!";
                                PnlSaran.Background = Brushes.ForestGreen; // Hijau yang lebih cerah
                            }
                        }
                    }
                    else
                    {
                        LblSuhu.Text = "[API Error] Gagal mendapatkan data cuaca.";
                        LblSaran.Text = "Saran: Tetap ikuti lokasi kumpul utama.";
                    }
                }
            }
            catch (Exception ex)
            {
                LblSuhu.Text = $"[API Error] Koneksi bermasalah: {ex.Message}";
                LblSaran.Text = "Saran: Mode Offline. Ikuti lokasi kumpul utama.";
            }
        }
    }
}