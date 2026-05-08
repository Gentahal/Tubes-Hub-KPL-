using System;

namespace TubesHub.ModulProgress
{
    // Ubah nama menjadi TaskState agar tidak bentrok
    public enum TaskState
    {
        ToDo,
        InProgress,
        Done
    }

    public class TaskItem
    {
        public string Title { get; private set; }
        public TaskState CurrentState { get; private set; } // Ubah tipe datanya
        public int Progress { get; private set; }

        public TaskItem(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Error: Judul tugas tidak boleh kosong.");

            Title = title;
            CurrentState = TaskState.ToDo;
            Progress = 0;
        }

        public void TransitionTo(TaskState nextState) // Ubah parameternya
        {
            bool isValidTransition = false;

            if (CurrentState == TaskState.ToDo && nextState == TaskState.InProgress)
            {
                isValidTransition = true;
            }
            else if (CurrentState == TaskState.InProgress && nextState == TaskState.Done)
            {
                if (Progress < 100)
                    throw new InvalidOperationException($"Error: Tugas '{Title}' belum mencapai 100%, tidak bisa diubah ke Done.");
                
                isValidTransition = true;
            }

            if (isValidTransition)
            {
                Console.WriteLine($"[State Change] '{Title}': {CurrentState} -> {nextState}");
                CurrentState = nextState;
            }
            else
            {
                throw new InvalidOperationException($"Error: Transisi dari {CurrentState} ke {nextState} tidak diperbolehkan.");
            }
        }

        public void UpdateProgress(int newProgress)
        {
            if (newProgress < 0 || newProgress > 100)
                throw new ArgumentOutOfRangeException(nameof(newProgress), "Error: Progress harus di antara 0 hingga 100.");

            if (CurrentState == TaskState.ToDo && newProgress > 0)
                throw new InvalidOperationException("Error: Ubah status ke 'In Progress' terlebih dahulu.");

            Progress = newProgress;
            Console.WriteLine($"[Update] Progress '{Title}' sekarang: {Progress}%");
        }
    }
}