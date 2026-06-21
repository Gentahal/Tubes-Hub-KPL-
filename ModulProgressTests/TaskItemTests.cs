using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TubesHub.ModulProgress;

namespace ModulProgressTests
{
    [TestClass]
    public class TaskItemTests
    {
        [TestMethod]
        public void Transition_ToDoToInProgress_ShouldSucceed()
        {
            var task = new TaskItem("Test Task");

            task.TransitionTo(TaskState.InProgress);

            Assert.AreEqual(TaskState.InProgress, task.CurrentState);
        }

        [TestMethod]
        public void Transition_ToDone_TanpaProgress100_ShouldThrowException()
        {
            var task = new TaskItem("Test Task");
            task.TransitionTo(TaskState.InProgress);
            task.UpdateProgress(50); 

            NUnit.Framework.Assert.Throws<InvalidOperationException>(() => task.TransitionTo(TaskState.Done));
        }

        [TestMethod]
        public void UpdateProgress_InputLebihDari100_ShouldThrowException()
        {
            var task = new TaskItem("Test Task");
            task.TransitionTo(TaskState.InProgress);

            NUnit.Framework.Assert.Throws<ArgumentOutOfRangeException>(() => task.UpdateProgress(150));
        }
    }
}