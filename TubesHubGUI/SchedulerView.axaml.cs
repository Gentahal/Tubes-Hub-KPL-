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
        // Jadwal kumpul kelompok berdasarkan indeks hari (0 = Senin, dst.)
        private static readonly string[] LokasiKumpul = {
            "Laboratorium KPL", "Kantin Teknik", "Perpustakaan",
            "Discord (Online)", "Cafe", "Libur", "Libur"
        };

        public SchedulerView()
        {
            InitializeComponent();
            var cmb = this.FindControl<ComboBox>("CmbHari");
            if (cmb != null) cmb.SelectedIndex = 0;
        }

        private void BtnCek_Click(object sender, RoutedEventArgs e)
        {
            var cmbHari = this.FindControl<ComboBox>("CmbHari");
            var lblJudulHasil = this.FindControl<TextBlock>("LblJudulHasil");
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

            if (lblJudulHasil != null)
            {
                var selectedItem = cmbHari.SelectedItem as ComboBoxItem;
                string namaHari = (selectedItem != null && selectedItem.Content != null)
                    ? selectedItem.Content.ToString() ?? "Dipilih"
                    : "Dipilih";
                lblJudulHasil.Text = $"Hasil Pengecekan Hari {namaHari}:";
            }

            int indeksHari = cmbHari.SelectedIndex;
            string lokasiHariIni = LokasiKumpul[indeksHari];

            if (lblLokasi != null) lblLokasi.Text = "Memproses lokasi...";
            if (lblSuhu != null) lblSuhu.Text = "Mengambil data cuaca...";
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

                            if (lblSuhu != null) lblSuhu.Text = $"Suhu saat ini: {suhu}C.";

                            // KONDISI 1: Mengecek jika hari libur (Sabtu/Minggu)
                            if (lokasiHariIni == "Libur")
                            {
                                if (lblLokasi != null) lblLokasi.Text = "Jadwal Hari Ini:\nLibur (Selamat Berakhir Pekan!)";
                                if (lblSaran != null) lblSaran.Text = "Saran: Hari ini jadwalnya libur, silakan kerjakan tugas mandiri atau istirahat.";
                                if (pnlSaran != null) pnlSaran.Background = Brushes.DarkGreen;
                            }
                            // KONDISI 2: Mengecek jika cuaca panas (Opsi A - Mengarahkan langsung ke tempat adem)
                            else if (suhu > 30)
                            {
                                if (lblLokasi != null)
                                {
                                    lblLokasi.Text = "Lokasi Alternatif (Cuaca Panas):\nRuangan Ber-AC atau Discord (Online)";
                                }
                                if (lblSaran != null)
                                {
                                    lblSaran.Text = "Saran: Suhu luar ruangan terlalu terik, diskusi tatap muka di area terbuka sangat tidak disarankan.";
                                }
                                if (pnlSaran != null)
                                {
                                    pnlSaran.Background = Brushes.Crimson;
                                }
                            }
                            // KONDISI 3: Cuaca adem / mendukung
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
                                if (lblLokasi != null) lblLokasi.Text = $"Semua Tempat Tersedia (Tatap Muka):\n{semuaTempatAman}";
                            }
                        }
                    }
                    else
                    {
                        if (lblSuhu != null) lblSuhu.Text = "[API Error] Gagal mendapatkan data cuaca.";
                        if (lblLokasi != null) lblLokasi.Text = $"Lokasi kumpul utama: {lokasiHariIni}";
                        if (lblSaran != null) lblSaran.Text = "Saran: Tetap ikuti lokasi kumpul utama.";
                    }
                }
            }
            catch (Exception ex)
            {
                if (lblSuhu != null) lblSuhu.Text = $"[API Error] Koneksi bermasalah: {ex.Message}";
                if (lblLokasi != null) lblLokasi.Text = $"Lokasi kumpul utama: {lokasiHariIni}";
                if (lblSaran != null) lblSaran.Text = "Saran: Mode Offline. Ikuti lokasi kumpul utama.";
            }
        }
    }
}