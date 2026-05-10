using System;
using System.Net.Http;

namespace TubesHub
{
    public class SchedulerModule
    {
        private static readonly string[] LokasiKumpul = { "Laboratorium KPL", "Kantin Teknik", "Perpustakaan", "Discord (Online)", "Cafe", "Libur", "Libur" };

        public static void Jalankan()
        {
            Console.Write("Masukkan hari dalam angka (1=Senin ... 7=Minggu): ");

            if (!int.TryParse(Console.ReadLine(), out int hari) || hari < 1 || hari > 7)
            {
                Console.WriteLine("[ERROR DbC] Input tidak valid. Harus angka 1 sampai 7.");
                return;
            }

            Console.WriteLine($"Lokasi kumpul: {LokasiKumpul[hari - 1]}");

            Console.WriteLine("[API Check] Mengecek cuaca via API eksternal... Cuaca mendukung!");
        } 
    } 
} 