using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace TubesHub
{
    public class FinalisasiModule
    {
        private static readonly string[] ValidStates = { "Draft", "Revisi", "Siap Kumpul" };

        public static void Jalankan()
        {
            string filePath = "laporan.json"; 

            if (!File.Exists(filePath))
            {
                Console.WriteLine("[ERROR DbC] File laporan.json hilang!");
                return;
            }

            string konten = File.ReadAllText(filePath);
            Console.WriteLine("Status dokumen saat ini (dari Config):");
            Console.WriteLine(konten);

            var dataLaporan = JsonSerializer.Deserialize<Dictionary<string, string>>(konten);

            Console.Write("\nPilih Bab yang ingin diupdate (Bab1/Bab2/Bab3): ");
            string bab = Console.ReadLine() ?? "";

            if (dataLaporan == null || !dataLaporan.ContainsKey(bab))
            {
                Console.WriteLine("[ERROR DbC] Bab tidak ditemukan dalam konfigurasi laporan.json!");
                return;
            }

            Console.Write($"Ubah status {bab} menjadi (Draft/Revisi/Siap Kumpul): ");
            string input = Console.ReadLine() ?? "";

            if (Array.IndexOf(ValidStates, input) == -1)
            {
                Console.WriteLine("[ERROR DbC] Status tidak valid! Automata menolak transisi state.");
            }
            else
            {
                Console.WriteLine($"Status {bab} berhasil bertransisi ke state: {input}");

                dataLaporan[bab] = input;
                string updatedJson = JsonSerializer.Serialize(dataLaporan, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, updatedJson);

                Console.WriteLine("[INFO] File laporan.json berhasil diperbarui!");
            }
        }
    }
}