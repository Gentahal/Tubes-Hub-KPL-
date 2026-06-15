using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TubesHub.ModulProgress; 

namespace ModulProgressTests
{
    [TestClass]
    public class ProgressModuleTests
    {
        [TestMethod]
        public void Automata_TransisiValid_ToDoKeInProgress_HarusBerhasil()
        {
            TaskItem task = new TaskItem("Implementasi Fitur API");

            task.TransitionTo(TaskState.InProgress);

            Assert.AreEqual(TaskState.InProgress, task.CurrentState, "State Automata gagal berubah ke InProgress.");
        }

        [TestMethod]
        public void DbC_Automata_TransisiInvalid_ToDoLangsungKeDone_HarusDitolak()
        {
            TaskItem task = new TaskItem("Implementasi UI");
            
            Assert.ThrowsException<InvalidOperationException>(() => 
            {
                task.TransitionTo(TaskState.Done);
            }, "DbC Gagal: Automata seharusnya menolak transisi ilegal dari ToDo langsung ke Done!");
        }

        [TestMethod]
        public async Task TesPerforma_PengecekanAPI_TidakBolehLebihDari2Detik()
        {
            DateTime tanggalTes = new DateTime(2026, 8, 17);
            Stopwatch sw = new Stopwatch();

            sw.Start();
            bool isLibur = await HolidayChecker.IsHolidayAsync(tanggalTes);
            sw.Stop();

            long waktuEksekusi = sw.ElapsedMilliseconds;
            
            Assert.IsTrue(waktuEksekusi < 2000, $"Performa lambat! Pemanggilan API memakan waktu: {waktuEksekusi} ms");
        }
    }
}