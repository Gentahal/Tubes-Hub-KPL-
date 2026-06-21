using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using tubes_hub.Tubes_Hub_KPL_; 

namespace TubesHub.Tests
{
    [TestClass]
    public class WbsModuleTests
    {
        [TestMethod]
        public void Inisialisasi_WBSModule_BulanValid_HarusBerhasil()
        {
            int bulanMulai = 5; 

            WBSModule wbs = new WBSModule(bulanMulai);

            Assert.IsNotNull(wbs, "Objek WBSModule gagal dibuat/diinisialisasi.");
        }

        [TestMethod]
        public void DbC_CekKategori_InputKosong_HarusDitolak()
        {
            WBSModule wbs = new WBSModule(1);
            
            NUnit.Framework.Assert.Throws<ArgumentException>(() => 
            {
                wbs.AddTask("", "Bikin Halaman Login", "Deskripsi login");
            }, "DbC Gagal: Program seharusnya menolak dan error saat kategori kosong!");
        }

        [TestMethod]
        public void TesPerforma_EksekusiValidasi_TidakBolehLebihDari100ms()
        {
            WBSModule wbs = new WBSModule(1);
            Stopwatch sw = new Stopwatch();

            sw.Start();  
            
            wbs.AddTask("UI", "Halaman Utama", "Membuat dashboard UI");
            
            sw.Stop();

            long waktuEksekusi = sw.ElapsedMilliseconds;
            Assert.IsTrue(waktuEksekusi < 100, $"Performa lambat! Waktu eksekusi: {waktuEksekusi} ms");
        }
    }
}