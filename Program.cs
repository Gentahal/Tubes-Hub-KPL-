using System;
using System.Diagnostics; // Wajib ditambahkan untuk mengukur waktu (Performance Testing)
using System.Threading.Tasks;
using TubesHub.ModulProgress;

namespace TubesHub
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Simulasi Modul Progress & API (Tubes Hub) ===");
            Console.WriteLine("-------------------------------------------------\n");

            // --- PERFORMANCE TESTING & TEST API HARI LIBUR ---
            Console.WriteLine(">>> Menguji API (Tanggal Libur) sekaligus Performance Testing...");
            DateTime liburNasional = new DateTime(2026, 8, 17);
            
            Stopwatch timer = new Stopwatch(); // 1. Siapkan alat ukur
            timer.Start();                     // 2. Mulai hitung milidetik

            bool isLibur1 = await HolidayChecker.IsHolidayAsync(liburNasional);
            
            timer.Stop();                      // 3. Matikan hitungan setelah API merespons

            // Tampilkan hasil Performance Testing
            Console.WriteLine($"[Performance] Waktu respons API: {timer.ElapsedMilliseconds} ms");
            Console.WriteLine($"Hasil pengecekan bool: {isLibur1}\n");


            // --- TEST API TANGGAL BIASA ---
            Console.WriteLine(">>> Menguji API (Tanggal Biasa)");
            DateTime hariBiasa = new DateTime(2026, 5, 10); 
            bool isLibur2 = await HolidayChecker.IsHolidayAsync(hariBiasa);
            Console.WriteLine($"Hasil pengecekan bool: {isLibur2}\n");


            // --- TEST AUTOMATA ---
            Console.WriteLine(">>> Menguji Automata Progress");
            try 
            {
                TaskItem task1 = new TaskItem("Integrasi UI Terminal");
                task1.TransitionTo(TaskState.InProgress);
                task1.UpdateProgress(30);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}