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
        } // Kurung tutup untuk fungsi Jalankan()
    } // Kurung tutup untuk class
} // Kurung tutup untuk namespace