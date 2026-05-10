using System;
using System.IO;
using System.Text.Json; // Teknik 2: Code Reuse / Library

namespace TubesHub
{
    public class TeamInfoModule
    {
        public static void Jalankan()
        {
            string filePath = "team.json"; // Teknik 1: Runtime Configuration

            // Defensive Programming (DbC): Mencegah error jika file tidak ada
            if (!File.Exists(filePath))
            {
                Console.WriteLine("[ERROR DbC] File konfigurasi team.json tidak ditemukan!");
                return;
            }

            string jsonString = File.ReadAllText(filePath);

            // Menggunakan library untuk parsing
            using (JsonDocument document = JsonDocument.Parse(jsonString))
            {
                Console.WriteLine("=== Daftar Tim Tubes Hub ===");
                foreach (JsonElement element in document.RootElement.EnumerateArray())
                {
                    string nama = element.GetProperty("Nama").GetString();
                    string role = element.GetProperty("Role").GetString();
                    Console.WriteLine($"- {nama} (Tugas: {role})");
                }
            }
        }
    }
}