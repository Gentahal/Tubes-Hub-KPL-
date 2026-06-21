using System;

namespace TubesHub
{
    public enum TaskState
    {
        ToDo,
        InProgress,
        Done
    }

    public class TeamMember
    {
        public string Nim { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class UnifiedTask
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Category { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        
        public int Weight { get; set; }
        public string BobotLevel { get; set; } = string.Empty;
        public int EstimatedDays { get; set; }
        public int RelativeMonth { get; set; }

        public TaskState CurrentState { get; set; } = TaskState.ToDo;
        public int Progress { get; set; } = 0;
        public DocumentState DocState { get; set; } = DocumentState.Draft;
        
        public string AssignedTo { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }

        public void TransitionTo(TaskState nextState)
        {
            if (CurrentState == nextState)
            {
                return; // do nothing
            }

            if (nextState == TaskState.Done && Progress < 100)
            {
                throw new InvalidOperationException($"[Automata Error] Tugas '{Title}' belum mencapai 100%, tidak bisa diubah ke Done.");
            }

            if (nextState == TaskState.ToDo)
            {
                Progress = 0; // Force progress to 0 if reverted to To Do
            }

            // Allow any valid logical transition for flexible editing
            CurrentState = nextState;
        }

        public void UpdateProgress(int newProgress)
        {
            if (newProgress < 0 || newProgress > 100)
                throw new ArgumentOutOfRangeException(nameof(newProgress), "[DbC Error] Progress harus di antara 0 hingga 100 (Defensive Programming).");

            Progress = newProgress;
            
            // Auto transition state based on progress
            if (Progress > 0 && Progress < 100 && CurrentState == TaskState.ToDo)
            {
                CurrentState = TaskState.InProgress;
            }
            else if (Progress == 100 && CurrentState != TaskState.Done)
            {
                CurrentState = TaskState.Done;
            }
            else if (Progress == 0 && CurrentState != TaskState.ToDo)
            {
                CurrentState = TaskState.ToDo;
            }
        }
    }
}
