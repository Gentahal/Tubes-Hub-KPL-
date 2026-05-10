using System;
using System.Net.Http;

namespace TubesHub
{
    public class SchedulerModule
    {
        // Teknik 1: Table Driven Construction (Index 0=Senin, 1=Selasa, dst)
        private static readonly string[] LokasiKumpul = { "Laboratorium KPL", "Kantin Teknik", "Perpustakaan", "Discord (Online)", "Cafe", "Libur", "Libur" };

        public static void Jalankan()
        {
            Console.Write("Masukkan hari dalam angka (1=Senin ... 7=Minggu): ");

            // Defensive Programming (DbC): Validasi Input Numerik dan Range
            if (!int.TryParse(Console.ReadLine(), out int hari) || hari < 1 || hari > 7)
            {
                Console.WriteLine("[ERROR DbC] Input tidak valid. Harus angka 1 sampai 7.");
                return;
            }

            Console.WriteLine($"Lokasi kumpul: {LokasiKumpul[hari - 1]}");

            // Teknik 2: API (Simulasi Cek Cuaca sederhana)
            Console.WriteLine("[API Check] Mengecek cuaca via API eksternal... Cuaca mendukung!");
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