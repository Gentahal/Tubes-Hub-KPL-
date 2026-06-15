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
        // Menyinkronkan array LokasiKumpul persis seperti di backend konsol kalian
        private static readonly string[] LokasiKumpul = {
            "Laboratorium KPL", "Kantin Teknik", "Perpustakaan",
            "Discord (Online)", "Cafe", "Libur", "Libur"
        };

        public SchedulerView()
        {
            // Menggunakan inisialisasi standar bawaan Avalonia compiler
            InitializeComponent();

            // Set default pilihan ComboBox ke hari Senin (indeks 0) saat program dibuka
            var cmb = this.FindControl<ComboBox>("CmbHari");
            if (cmb != null) cmb.SelectedIndex = 0;
        }

        private void BtnCek_Click(object sender, RoutedEventArgs e)
        {
            // Mencari komponen secara dinamis untuk menghindari 'ghost error' di teks editor
            var cmbHari = this.FindControl<ComboBox>("CmbHari");
            var lblLokasi = this.FindControl<TextBlock>("LblLokasi");
            var lblSuhu = this.FindControl<TextBlock>("LblSuhu");
            var lblSaran = this.FindControl<TextBlock>("LblSaran");
            var pnlSaran = this.FindControl<Border>("PnlSaran");

            // Validasi Input (Defensive UX menggantikan int.TryParse di konsol)
            if (cmbHari == null || cmbHari.SelectedIndex == -1)
            {
                if (lblSaran != null) lblSaran.Text = "[ERROR] Silakan pilih hari terlebih dahulu.";
                return;
            }

            //  Ekstrak data menggunakan Table-Driven berdasarkan pilihan ComboBox
            int indeksHari = cmbHari.SelectedIndex;
            string lokasi = LokasiKumpul[indeksHari];

            if (lblLokasi != null) lblLokasi.Text = $"Lokasi kumpul: {lokasi}";

            //Mengecek cuaca di Telkom University via API eksternal (Open-Meteo)
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

                            if (lblSuhu != null) lblSuhu.Text = $"[API Berhasil] Suhu saat ini: {suhu}°C.";

                            // Logika Keputusan (Sesuai dengan teks backend konsol kelompokmu)
                            if (suhu > 30)
                            {
                                if (lblSaran != null) lblSaran.Text = "Saran: Cuaca cukup panas, sebaiknya kumpul di ruangan ber-AC atau via Discord.";
                                if (pnlSaran != null) pnlSaran.Background = Brushes.Crimson; // Merah jika panas
                            }
                            else
                            {
                                if (lblSaran != null) lblSaran.Text = "Saran: Cuaca sangat mendukung untuk diskusi tatap muka!";
                                if (pnlSaran != null) pnlSaran.Background = Brushes.DarkGreen; // Hijau jika sejuk
                            }
                        }
                    }
                    else
                    {
                        if (lblSuhu != null) lblSuhu.Text = "[API Error] Gagal mendapatkan data cuaca.";
                        if (lblSaran != null) lblSaran.Text = "Saran: Tetap ikuti lokasi kumpul utama.";
                        if (pnlSaran != null) pnlSaran.Background = Brushes.Gray;
                    }
                }
            }
            catch (Exception ex)
            {
                // Menangkap exception koneksi bermasalah persis seperti blok catch di konsol kalian
                if (lblSuhu != null) lblSuhu.Text = $"[API Error] Koneksi bermasalah: {ex.Message}";
                if (lblSaran != null) lblSaran.Text = "Saran: Mode Offline. Ikuti lokasi kumpul utama.";
                if (pnlSaran != null) pnlSaran.Background = Brushes.Gray;
            }
        }
    }
}