using System;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic; // Ditambahkan agar sinkron dengan struktur list filter di GUI

namespace TubesHub
{
    public class SchedulerModule
    {
        private static readonly string[] LokasiKumpul = { "Laboratorium KPL", "Kantin Teknik", "Perpustakaan", "Discord (Online)", "Cafe", "Libur", "Libur" };

        public static void Jalankan()
        {
            Console.Write("Masukkan hari dalam angka (1=Senin ... 7=Minggu): ");

            if (!int.TryParse(Console.ReadLine(), out int hari) || hari < 1 || hari > 7)
            {
                Console.WriteLine("[ERROR DbC] Input tidak valid. Harus angka 1 sampai 7.");
                return;
            }

            Console.WriteLine("[API Check] Mengecek cuaca di Telkom University via API eksternal...");
            CekCuacaAsli(hari);
        }

        private static void CekCuacaAsli(int hari)
        {
            string lokasiHariIni = LokasiKumpul[hari - 1];
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

                            Console.WriteLine($"[API Berhasil] Suhu saat ini: {suhu}°C.");

                            if (suhu > 30)
                            {
                                Console.WriteLine($"📍 Lokasi kumpul hari ini: {lokasiHariIni}");
                                Console.WriteLine("Saran: Cuaca cukup panas, sebaiknya kumpul di ruangan ber-AC atau via Discord.");
                            }
                            else
                            {
                                Console.WriteLine("Saran: Cuaca sangat mendukung untuk diskusi tatap muka!");

                                // Sinkronisasi Logika: Tampilkan semua lokasi comfy seragam di console
                                List<string> tempatTatapMuka = new List<string>();
                                foreach (string tempat in LokasiKumpul)
                                {
                                    if (tempat != "Discord (Online)" && tempat != "Libur" && !tempatTatapMuka.Contains(tempat))
                                    {
                                        tempatTatapMuka.Add(tempat);
                                    }
                                }
                                string semuaTempatAman = string.Join(", ", tempatTatapMuka);
                                Console.WriteLine($"✨ Semua Tempat Tersedia (Tatap Muka):\n👉 {semuaTempatAman}");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"📍 Lokasi kumpul utama: {lokasiHariIni}");
                        Console.WriteLine("[API Error] Gagal mendapatkan data cuaca.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"📍 Lokasi kumpul utama: {lokasiHariIni}");
                Console.WriteLine($"[API Error] Koneksi bermasalah: {ex.Message}");
            }
        }
    }
}