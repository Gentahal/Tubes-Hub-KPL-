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
            var task = new TaskItem("Test Task");

            task.TransitionTo(TaskState.InProgress);

            Assert.Equal(TaskState.InProgress, task.CurrentState);
        }

        [Fact]
        public void Transition_ToDone_TanpaProgress100_ShouldThrowException()
        {
            var task = new TaskItem("Test Task");
            task.TransitionTo(TaskState.InProgress);
            task.UpdateProgress(50); 

            Assert.Throws<InvalidOperationException>(() => task.TransitionTo(TaskState.Done));
        }

        [Fact]
        public void UpdateProgress_InputLebihDari100_ShouldThrowException()
        {
            var task = new TaskItem("Test Task");
            task.TransitionTo(TaskState.InProgress);

            Assert.Throws<ArgumentOutOfRangeException>(() => task.UpdateProgress(150));
        }
    }
}