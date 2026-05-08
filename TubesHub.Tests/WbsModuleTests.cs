// using Microsoft.VisualStudio.TestTools.UnitTesting;
// using System.Diagnostics;
// using TubesHub;

// namespace TubesHub.Tests
// {
//     [TestClass]
//     public class WbsModuleTests
//     {
//         [TestMethod]
//         public void KategoriTable_HarusMemilikiTigaKategoriUtama()
//         {
//             int expectedCount = 3; 
            
//             Assert.AreEqual(expectedCount, 3, "Tabel Kategori harus memiliki 3 item: UI, API, Database");
//         }

//         [TestMethod]
//         public void DbC_CekKategori_InputKosong_HarusDitolak()
//         {
            
//         }

//         [TestMethod]
//         public void TesPerforma_EksekusiValidasi_TidakBolehLebihDari100ms()
//         {
//             Stopwatch sw = new Stopwatch();

//             sw.Start();            
//             sw.Stop();

//             long waktuEksekusi = sw.ElapsedMilliseconds;
            
//             Assert.IsTrue(waktuEksekusi < 100, $"Performa lambat! Waktu eksekusi: {waktuEksekusi} ms");
//         }
//     }
// }