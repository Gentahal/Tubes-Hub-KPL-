using System;
using System.IO;
using System.Text.Json; 

namespace TubesHub
{
    public class TeamInfoModule
    {
        public static void Jalankan()
        {
            string filePath = "team.json"; 

            if (!File.Exists(filePath))
            {
                Console.WriteLine("[ERROR DbC] File konfigurasi team.json tidak ditemukan!");
                return;
            }

            string jsonString = File.ReadAllText(filePath);

            using (JsonDocument document = JsonDocument.Parse(jsonString))
            {
                Console.WriteLine("=== Daftar Tim Tubes Hub ===");
                foreach (JsonElement element in document.RootElement.EnumerateArray())
                {
                    string? nama = element.GetProperty("Nama").GetString();
                    string? role = element.GetProperty("Role").GetString();
                    Console.WriteLine($"- {nama} (Tugas: {role})");
                }
            }
        }
    }
}