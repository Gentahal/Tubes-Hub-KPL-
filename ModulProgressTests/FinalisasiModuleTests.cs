using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TubesHubGUI;

namespace ModulProgressTests
{
    [TestClass]
    public class FinalisasiModuleTests
    {
        private readonly string testFilePath = "laporan.json";

        [TestInitialize]
        public void SetUp()
        {
            var dummyData = new Dictionary<string, string>
            {
                { "Bab 1", "Draft" },
                { "Bab 2", "Revisi" }
            };
            string json = System.Text.Json.JsonSerializer.Serialize(dummyData);
            File.WriteAllText(testFilePath, json);
        }

        [TestCleanup]
        public void TearDown()
        {
            if (File.Exists(testFilePath))
            {
                File.Delete(testFilePath);
            }
        }

        [TestMethod]
        public void UbahStatusDokumen_BabValidDanStatusValid_HarusBerhasil()
        {
            FinalisasiModule.UbahStatusDokumen("Bab 1", "Siap Kumpul");

            var updatedData = FinalisasiModule.GetLaporanStatus();
            Assert.AreEqual("Siap Kumpul", updatedData["Bab 1"], "Status gagal diubah menjadi Siap Kumpul.");
        }

        [TestMethod]
        public void DbC_UbahStatusDokumen_BabTidakDitemukan_HarusDitolak()
        {
            NUnit.Framework.Assert.Throws<ArgumentException>(() => 
            {
                FinalisasiModule.UbahStatusDokumen("Bab 3", "Revisi");
            }, "DbC Gagal: Bab tidak ditemukan seharusnya menolak dengan ArgumentException.");
        }

        [TestMethod]
        public void DbC_UbahStatusDokumen_StatusTidakValid_HarusDitolak()
        {
            NUnit.Framework.Assert.Throws<InvalidOperationException>(() => 
            {
                FinalisasiModule.UbahStatusDokumen("Bab 2", "Selesai");
            }, "DbC Gagal: Status tidak valid seharusnya menolak dengan InvalidOperationException.");
        }

        [TestMethod]
        public void TesPerforma_UbahStatusDokumen_TidakBolehLebihDari100ms()
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();
            FinalisasiModule.UbahStatusDokumen("Bab 1", "Revisi");
            sw.Stop();

            long waktuEksekusi = sw.ElapsedMilliseconds;
            Assert.IsTrue(waktuEksekusi < 100, $"Performa lambat! Waktu eksekusi: {waktuEksekusi} ms");
        }
    }
}
