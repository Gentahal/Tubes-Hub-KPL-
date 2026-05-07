using System;
using tubes_hub.Tubes_Hub_KPL_;

namespace tubes_hub.Tubes_Hub_KPL_
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== INISIALISASI PROYEK TUBES HUB ===");
            Console.Write("Proyek dimulai bulan ke (1-12): ");
            int startMonth;
            while (!int.TryParse(Console.ReadLine(), out startMonth) || startMonth < 1 || startMonth > 12)
            {
                Console.Write("Input tidak valid. Masukkan angka 1-12: ");
            }

            WBSModule wbs = new WBSModule(startMonth);
            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\n--- MENU UTAMA WBS ---");
                Console.WriteLine("1. Tambah Tugas Baru");
                Console.WriteLine("2. Lihat Timeline Perencanaan");
                Console.WriteLine("3. Keluar");
                Console.Write("Pilih: ");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.WriteLine("\nPILIH KATEGORI TUGAS:");
                    Console.WriteLine("1. UI");
                    Console.WriteLine("2. Backend");
                    Console.WriteLine("3. Database");
                    Console.WriteLine("4. Dokumentasi");
                    Console.WriteLine("5. Testing");
                    Console.Write("Pilih (1-5): ");

                    string catChoice = Console.ReadLine();
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
                    string title = Console.ReadLine();
                    Console.Write("Masukkan Deskripsi Detail: ");
                    string desc = Console.ReadLine();

                    wbs.AddTask(selectedCategory, title, desc);
                }
                else if (choice == "2")
                {
                    wbs.ShowWBSPlan();
                }
                else if (choice == "3")
                {
                    isRunning = false;
                }
            }
        }
    }
}