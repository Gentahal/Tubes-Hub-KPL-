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
                Console.WriteLine("1. Jalankan Modul Progress");
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
                        Console.WriteLine("[INFO] Modul Finalisasi belum tersedia di console app ini.");
                        break;
                    case "0":
                        isProgramRunning = false;
                        Console.WriteLine("Terima kasih telah menggunakan Tubes Hub!");
                        break;
                    default:
                        Console.WriteLine("Pilihan tidak valid!");
                        break;
                }
            }
        }

        static async Task TestModulProgress()
        {
            TaskItem? activeTask = null;
            bool isModulProgressRunning = true;

            while (isModulProgressRunning)
            {
                Console.WriteLine("\n=========================================");
                Console.WriteLine("    TUBES HUB - MODUL PROGRESS (DEMO)    ");
                Console.WriteLine("=========================================");
                Console.WriteLine("1. Buat Tugas Baru");
                Console.WriteLine("2. Cek Deadline Tugas (API Hari Libur)");
                Console.WriteLine("3. Ubah Status Tugas (Automata)");
                Console.WriteLine("4. Update Persentase Progress (DbC)");
                Console.WriteLine("5. Kembali ke Menu Utama");
                Console.WriteLine("=========================================");

                if (activeTask != null)
                {
                    Console.WriteLine($"[Tugas Aktif]: {activeTask.Title} | Status: {activeTask.CurrentState} | Progress: {activeTask.Progress}%");
                    Console.WriteLine("-----------------------------------------");
                }

                Console.Write("Pilih aksi (1-5): ");
                string pilihan = Console.ReadLine() ?? "";
                Console.WriteLine();

                switch (pilihan)
                {
                    case "1":
                        Console.Write("Masukkan nama tugas baru: ");
                        string namaTugas = Console.ReadLine() ?? "";
                        try
                        {
                            activeTask = new TaskItem(namaTugas);
                            Console.WriteLine($"[Sukses] Tugas '{activeTask.Title}' berhasil dibuat.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[Error DbC] {ex.Message}");
                        }
                        break;

                    case "2":
                        Console.Write("Masukkan tanggal deadline (Format: YYYY-MM-DD, contoh: 2026-08-17): ");
                        string inputTanggal = Console.ReadLine() ?? "";

                        if (DateTime.TryParse(inputTanggal, out DateTime deadlineDate))
                        {
                            Console.WriteLine("\nMemeriksa kalender API...");

                            // Performance Testing
                            Stopwatch timer = new Stopwatch();
                            timer.Start();

                            bool isLibur = await HolidayChecker.IsHolidayAsync(deadlineDate);

                            timer.Stop();
                            Console.WriteLine($"[Performance] Waktu respons API: {timer.ElapsedMilliseconds} ms");
                        }
                        else
                        {
                            Console.WriteLine("[Error] Format tanggal salah. Gunakan format YYYY-MM-DD.");
                        }
                        break;

                    case "3":
                        if (activeTask == null)
                        {
                            Console.WriteLine("[Peringatan] Buat tugas baru terlebih dahulu (Menu 1)!");
                            break;
                        }

                        Console.WriteLine("Pilih target status baru:");
                        Console.WriteLine("0 = To Do");
                        Console.WriteLine("1 = In Progress");
                        Console.WriteLine("2 = Done");
                        Console.Write("Masukkan angka status (0/1/2): ");
                        string inputStatus = Console.ReadLine() ?? "";

                        try
                        {
                            TubesHub.ModulProgress.TaskState targetState = inputStatus switch
                            {
                                "0" => TubesHub.ModulProgress.TaskState.ToDo,
                                "1" => TubesHub.ModulProgress.TaskState.InProgress,
                                "2" => TubesHub.ModulProgress.TaskState.Done,
                                _ => throw new ArgumentException("Pilihan status tidak valid.")
                            };

                            activeTask.TransitionTo(targetState);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[Ditolak Automata] {ex.Message}");
                        }
                        break;

                    case "4":
                        if (activeTask == null)
                        {
                            Console.WriteLine("[Peringatan] Buat tugas baru terlebih dahulu (Menu 1)!");
                            break;
                        }

                        Console.Write("Masukkan persentase progress (0-100): ");
                        string inputProgress = Console.ReadLine() ?? "";

                        if (int.TryParse(inputProgress, out int persentase))
                        {
                            try
                            {
                                activeTask.UpdateProgress(persentase);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[Ditolak DbC] {ex.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("[Error] Input harus berupa angka bulat!");
                        }
                        break;

                    case "5":
                        isModulProgressRunning = false;
                        break;

                    default:
                        Console.WriteLine("[Error] Pilihan tidak valid. Silakan pilih 1-5.");
                        break;
                }

                if (isModulProgressRunning)
                {
                    Console.WriteLine("\nTekan ENTER untuk melanjutkan...");
                    Console.ReadLine();
                }
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
                    string? selectedCategory = catChoice switch
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