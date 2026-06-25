using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace TubesHub
{
    /// <summary>
    /// Modul Monitoring (Team Info).
    /// Teknik 1: Runtime Configuration -> membaca team.json di luar source code.
    /// Teknik 2: Code Reuse / Library -> memakai System.Text.Json untuk parsing.
    /// Tambahan: validasi NIM (DbC) dan kemampuan mengubah Role seseorang
    /// secara interaktif (di memori, tidak menulis ulang team.json, karena
    /// file ini juga dipakai oleh ProjectManager dan GUI Avalonia di tim).
    ///
    /// Catatan desain: Role di sini sengaja dipertahankan sebagai satu nilai
    /// (bukan daftar), agar tetap kompatibel dengan struktur TeamMember
    /// (Nim, Nama, Role) yang dipakai bersama oleh ProjectManager.cs dan
    /// TeamInfoView.axaml.cs.
    /// </summary>
    public class TeamInfoModule
    {
        private const string FilePath = "team.json";
        private const int PanjangNimValid = 12;

        private class Anggota
        {
            public string Nim = "";
            public string Nama = "";
            public string Role = "";
        }

        public static void Jalankan()
        {
            List<Anggota> anggotaList = MuatData();

            if (anggotaList.Count == 0)
            {
                Console.WriteLine("Tidak ada data anggota yang valid untuk ditampilkan.");
                return;
            }

            Console.WriteLine("[INFO] Perubahan Role di sesi ini hanya berlaku sementara (tidak menulis ulang team.json).");

            bool lanjut = true;
            while (lanjut)
            {
                Console.WriteLine("\n=== MENU TEAM INFO ===");
                Console.WriteLine("1. Tampilkan semua anggota");
                Console.WriteLine("2. Ubah Role seseorang");
                Console.WriteLine("0. Kembali ke Menu Utama");
                Console.Write("Pilih: ");
                string pilihan = Console.ReadLine() ?? "";

                switch (pilihan)
                {
                    case "1":
                        TampilkanSemua(anggotaList);
                        break;
                    case "2":
                        UbahRole(anggotaList);
                        break;
                    case "0":
                        lanjut = false;
                        break;
                    default:
                        Console.WriteLine("Pilihan tidak valid.");
                        break;
                }
            }
        }

        // --- Runtime Config + Code Reuse (System.Text.Json) ---
        private static List<Anggota> MuatData()
        {
            var hasil = new List<Anggota>();

            if (!File.Exists(FilePath))
            {
                Console.WriteLine($"[ERROR DbC] File konfigurasi '{FilePath}' tidak ditemukan!");
                return hasil;
            }

            string jsonString = File.ReadAllText(FilePath);

            try
            {
                using JsonDocument document = JsonDocument.Parse(jsonString);

                foreach (JsonElement el in document.RootElement.EnumerateArray())
                {
                    string nim = el.TryGetProperty("Nim", out var n) ? n.GetString() ?? "" : "";
                    string nama = el.TryGetProperty("Nama", out var nm) ? nm.GetString() ?? "" : "";
                    string role = el.TryGetProperty("Role", out var r) ? r.GetString() ?? "" : "";

                    if (string.IsNullOrWhiteSpace(nim) || string.IsNullOrWhiteSpace(nama) || string.IsNullOrWhiteSpace(role))
                    {
                        Console.WriteLine("[ERROR DbC] Ada entri dengan data tidak lengkap, dilewati.");
                        continue;
                    }

                    if (nim.Length != PanjangNimValid || !nim.All(char.IsDigit))
                    {
                        Console.WriteLine($"[ERROR DbC] NIM '{nim}' tidak valid (harus {PanjangNimValid} digit angka), dilewati.");
                        continue;
                    }

                    hasil.Add(new Anggota { Nim = nim, Nama = nama, Role = role });
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[ERROR] Format JSON tidak valid: {ex.Message}");
            }

            return hasil;
        }

        private static void TampilkanSemua(List<Anggota> anggotaList)
        {
            Console.WriteLine("\n=== Daftar Tim Tubes Hub ===");
            foreach (var a in anggotaList)
                Console.WriteLine($"- {a.Nama} ({a.Nim}) | Modul: {a.Role}");
            Console.WriteLine($"\nTotal anggota: {anggotaList.Count}");
        }

        private static void UbahRole(List<Anggota> anggotaList)
        {
            Console.Write("Masukkan Nama atau NIM anggota: ");
            string keyword = (Console.ReadLine() ?? "").Trim();

            var target = anggotaList.FirstOrDefault(a =>
                a.Nama.Contains(keyword, StringComparison.OrdinalIgnoreCase) || a.Nim == keyword);

            if (target == null)
            {
                Console.WriteLine($"[ERROR DbC] Anggota dengan kata kunci '{keyword}' tidak ditemukan.");
                return;
            }

            Console.WriteLine($"Role saat ini untuk {target.Nama}: {target.Role}");
            Console.Write("Masukkan Role baru: ");
            string roleBaru = (Console.ReadLine() ?? "").Trim();

            if (string.IsNullOrWhiteSpace(roleBaru))
            {
                Console.WriteLine("[ERROR DbC] Role tidak boleh kosong.");
                return;
            }

            if (roleBaru.Equals(target.Role, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"[INFO] Role {target.Nama} sudah '{roleBaru}', tidak ada perubahan.");
                return;
            }

            string roleLama = target.Role;
            target.Role = roleBaru;

            Console.WriteLine($"[Sukses] Role {target.Nama} diubah dari '{roleLama}' menjadi '{roleBaru}'.");
            Console.WriteLine("[INFO] Perubahan ini hanya berlaku untuk sesi program saat ini.");
        }
    }
}