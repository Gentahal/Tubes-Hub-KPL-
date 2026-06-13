using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace TubesHubGUI
{
    public class FinalisasiModule
    {
        private static readonly string[] ValidStates = { "Draft", "Revisi", "Siap Kumpul" };
        private static readonly string filePath = "laporan.json";

        public static Dictionary<string, string> GetLaporanStatus()
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("[ERROR DbC] File laporan.json hilang!");

            string konten = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(konten) ?? new Dictionary<string, string>();
        }

        public static void UbahStatusDokumen(string bab, string statusBaru)
        {
            var dataLaporan = GetLaporanStatus();

            if (!dataLaporan.ContainsKey(bab))
                throw new ArgumentException("[ERROR DbC] Bab tidak ditemukan dalam konfigurasi!");

            if (Array.IndexOf(ValidStates, statusBaru) == -1)
                throw new InvalidOperationException("[ERROR DbC] Status tidak valid! Automata menolak transisi state.");

            dataLaporan[bab] = statusBaru;
            string updatedJson = JsonSerializer.Serialize(dataLaporan, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, updatedJson);
        }
    }
}