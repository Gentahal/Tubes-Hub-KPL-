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
        
        public string AssignedTo { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }

        public void TransitionTo(TaskState nextState)
        {
            if (CurrentState == TaskState.ToDo && nextState == TaskState.InProgress)
            {
                CurrentState = nextState;
            }
            else if (CurrentState == TaskState.InProgress && nextState == TaskState.Done)
            {
                if (Progress < 100)
                    throw new InvalidOperationException($"[Automata Error] Tugas '{Title}' belum mencapai 100%, tidak bisa diubah ke Done.");
                
                CurrentState = nextState;
            }
            else if (CurrentState == nextState)
            {
                // do nothing
            }
            else
            {
                throw new InvalidOperationException($"[Automata Error] Transisi dari {CurrentState} ke {nextState} tidak diperbolehkan (Invalid State).");
            }
        }

        public void UpdateProgress(int newProgress)
        {
            if (newProgress < 0 || newProgress > 100)
                throw new ArgumentOutOfRangeException(nameof(newProgress), "[DbC Error] Progress harus di antara 0 hingga 100 (Defensive Programming).");

            if (CurrentState == TaskState.ToDo && newProgress > 0)
                throw new InvalidOperationException("[Automata Error] Ubah status ke 'In Progress' terlebih dahulu sebelum mengisi progress.");

            Progress = newProgress;
            
            // Auto complete if 100
            if (Progress == 100 && CurrentState == TaskState.InProgress)
            {
                CurrentState = TaskState.Done;
            }
        }
    }
}
