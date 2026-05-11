using System;
using System.Net.Http;
using System.Text.Json;

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

            Console.WriteLine($"Lokasi kumpul: {LokasiKumpul[hari - 1]}");

            Console.WriteLine("[API Check] Mengecek cuaca di Telkom University via API eksternal...");
            CekCuacaAsli();
        }

        private static void CekCuacaAsli()
        {
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
                                Console.WriteLine("Saran: Cuaca cukup panas, sebaiknya kumpul di ruangan ber-AC atau via Discord.");
                            }
                            else
                            {
                                Console.WriteLine("Saran: Cuaca sangat mendukung untuk diskusi tatap muka!");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("[API Error] Gagal mendapatkan data cuaca.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API Error] Koneksi bermasalah: {ex.Message}");
            }
        }
    }
}