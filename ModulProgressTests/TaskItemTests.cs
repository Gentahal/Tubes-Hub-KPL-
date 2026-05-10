using System;
using Xunit;
using TubesHub.ModulProgress;

namespace ModulProgressTests
{
    public class TaskItemTests
    {
        [Fact]
        public void Transition_ToDoToInProgress_ShouldSucceed()
        {
            // Arrange (Persiapan)
            var task = new TaskItem("Test Task");

            // Act (Aksi)
            task.TransitionTo(TaskState.InProgress);

            // Assert (Validasi)
            Assert.Equal(TaskState.InProgress, task.CurrentState);
        }

        [Fact]
        public void Transition_ToDone_TanpaProgress100_ShouldThrowException()
        {
            // Arrange
            var task = new TaskItem("Test Task");
            task.TransitionTo(TaskState.InProgress);
            task.UpdateProgress(50); // Sengaja baru 50%

            // Act & Assert (Harus menghasilkan error InvalidOperationException dari DbC)
            Assert.Throws<InvalidOperationException>(() => task.TransitionTo(TaskState.Done));
        }

        [Fact]
        public void UpdateProgress_InputLebihDari100_ShouldThrowException()
        {
            // Arrange
            var task = new TaskItem("Test Task");
            task.TransitionTo(TaskState.InProgress);

            // Act & Assert (Harus menghasilkan error ArgumentOutOfRangeException)
            Assert.Throws<ArgumentOutOfRangeException>(() => task.UpdateProgress(150));
        }
    }
}