using System;
using System.IO;

namespace TubesHub
{
    public class FinalisasiModule
    {
        private static readonly string[] ValidStates = { "Draft", "Revisi", "Siap Kumpul" };

        public static void Jalankan()
        {
            string filePath = "laporan.json"; 

            if (!File.Exists(filePath))
            {
                Console.WriteLine("[ERROR DbC] File laporan.json hilang!");
                return;
            }

            string konten = File.ReadAllText(filePath);
            Console.WriteLine("Status dokumen saat ini (dari Config):");
            Console.WriteLine(konten);

            Console.Write("Ubah status dokumen menjadi (Draft/Revisi/Siap Kumpul): ");
            string input = Console.ReadLine();

            if (Array.IndexOf(ValidStates, input) == -1)
            {
                Console.WriteLine("[ERROR DbC] Status tidak valid! Automata menolak transisi.");
            }
            else
            {
                Console.WriteLine($"Status dokumen berhasil bertransisi ke state: {input}");
            }
        }
    }
}