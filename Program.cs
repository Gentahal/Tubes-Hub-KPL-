using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TubesHub.ModulProgress;
using tubes_hub.Tubes_Hub_KPL_;

namespace TubesHub
{
    class Program
    {
        static async Task Main(string[] args)
        {
            bool isProgramRunning = true;
            while (isProgramRunning)
            {
                Console.WriteLine("\n=== MENU UTAMA TUBES HUB KPL ===");
                Console.WriteLine("1. Jalankan Modul Progress & API");
                Console.WriteLine("2. Jalankan Modul WBS");
                Console.WriteLine("3. Jalankan Modul Team Info");
                Console.WriteLine("4. Jalankan Modul Scheduler");
                Console.WriteLine("5. Jalankan Modul Finalisasi");
                Console.WriteLine("0. Keluar Aplikasi");
                Console.Write("Pilih menu: ");

                string mainChoice = Console.ReadLine() ?? "";
                switch (mainChoice)
                {
                    case "1":
                        await TestModulProgress();
                        break;
                    case "2":
                        TestModulWbs();
                        break;
                    case "3":
                        TeamInfoModule.Jalankan();
                        break;
                    case "4":
                        SchedulerModule.Jalankan();
                        break;
                    case "5":
                        FinalisasiModule.Jalankan();
                        break;
                    case "0":
                        isProgramRunning = false;
                        break;
                    default:
                        Console.WriteLine("Pilihan tidak valid!");
                        break;
                }
            }
        }

        static async Task TestModulProgress()
        {
            Console.WriteLine("\n=== Simulasi Modul Progress & API (Tubes Hub) ===");
            Console.WriteLine("-------------------------------------------------\n");

            Console.WriteLine(">>> Menguji API (Tanggal Libur) sekaligus Performance Testing...");
            DateTime liburNasional = new DateTime(2026, 8, 17);

            Stopwatch timer = new Stopwatch();
            timer.Start();

            bool isLibur1 = await HolidayChecker.IsHolidayAsync(liburNasional);

            timer.Stop();

            Console.WriteLine($"[Performance] Waktu respons API: {timer.ElapsedMilliseconds} ms");
            Console.WriteLine($"Hasil pengecekan bool: {isLibur1}\n");

            Console.WriteLine(">>> Menguji API (Tanggal Biasa)");
            DateTime hariBiasa = new DateTime(2026, 5, 10);
            bool isLibur2 = await HolidayChecker.IsHolidayAsync(hariBiasa);
            Console.WriteLine($"Hasil pengecekan bool: {isLibur2}\n");

            Console.WriteLine(">>> Menguji Automata Progress");
            try
            {
                TaskItem task1 = new TaskItem("Integrasi UI Terminal");
                task1.TransitionTo(TaskState.InProgress);
                task1.UpdateProgress(30);
                Console.WriteLine("Automata berjalan sukses.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void TestModulWbs()
        {
            Console.WriteLine("\n=== INISIALISASI PROYEK TUBES HUB ===");
            Console.Write("Proyek dimulai bulan ke (1-12): ");
            int startMonth;
            while (!int.TryParse(Console.ReadLine(), out startMonth) || startMonth < 1 || startMonth > 12)
            {
                Console.Write("Input tidak valid. Masukkan angka 1-12: ");
            }

            WBSModule wbs = new WBSModule(startMonth);
            bool isRunningWbs = true;

            while (isRunningWbs)
            {
                Console.WriteLine("\n--- MENU UTAMA WBS ---");
                Console.WriteLine("1. Tambah Tugas Baru");
                Console.WriteLine("2. Lihat Timeline Perencanaan");
                Console.WriteLine("3. Kembali ke Menu Utama");
                Console.Write("Pilih: ");
                string choice = Console.ReadLine() ?? "";

                if (choice == "1")
                {
                    Console.WriteLine("\nPILIH KATEGORI TUGAS:");
                    Console.WriteLine("1. UI");
                    Console.WriteLine("2. Backend");
                    Console.WriteLine("3. Database");
                    Console.WriteLine("4. Dokumentasi");
                    Console.WriteLine("5. Testing");
                    Console.Write("Pilih (1-5): ");

                    string catChoice = Console.ReadLine() ?? "";
                    string selectedCategory = catChoice switch
                    {
                        "1" => "UI",
                        "2" => "BACKEND",
                        "3" => "DATABASE",
                        "4" => "DOKUMENTASI",
                        "5" => "TESTING",
                        _ => null
                    };

                    if (selectedCategory == null)
                    {
                        Console.WriteLine("[ERROR] Pilihan kategori tidak valid.");
                        continue;
                    }

                    Console.Write("Masukkan Judul: ");
                    string title = Console.ReadLine() ?? "";
                    Console.Write("Masukkan Deskripsi Detail: ");
                    string desc = Console.ReadLine() ?? "";

                    wbs.AddTask(selectedCategory, title, desc);
                }
                else if (choice == "2")
                {
                    wbs.ShowWBSPlan();
                }
                else if (choice == "3")
                {
                    isRunningWbs = false;
                }
            }
        }
    }
}