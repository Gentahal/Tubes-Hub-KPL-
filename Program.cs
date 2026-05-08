using System;
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

            // --- TEST API HARI LIBUR ---
            Console.WriteLine(">>> Menguji API (Tanggal Libur Kemerdekaan RI)");
            DateTime liburNasional = new DateTime(2026, 8, 17);
            bool isLibur1 = await HolidayChecker.IsHolidayAsync(liburNasional);
            Console.WriteLine($"Hasil pengecekan bool: {isLibur1}\n");

            Console.WriteLine(">>> Menguji API (Tanggal Biasa)");
            DateTime hariBiasa = new DateTime(2026, 5, 10); 
            bool isLibur2 = await HolidayChecker.IsHolidayAsync(hariBiasa);
            Console.WriteLine($"Hasil pengecekan bool: {isLibur2}\n");

            // --- TEST AUTOMATA ---
            Console.WriteLine(">>> Menguji Automata Progress");
            try 
            {
                TaskItem task1 = new TaskItem("Integrasi UI Terminal");
                task1.TransitionTo(TaskState.InProgress); // Ubah di sini jadi TaskState
                task1.UpdateProgress(30);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}