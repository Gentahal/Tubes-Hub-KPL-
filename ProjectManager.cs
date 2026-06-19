using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace TubesHub
{
    public enum DocumentState
    {
        Draft,
        Revisi,
        SiapKumpul
    }

    public class DocumentAutomata
    {
        public DocumentState CurrentState { get; private set; } = DocumentState.Draft;

        public void TransitionTo(DocumentState nextState)
        {
            if (CurrentState == nextState) return;

            if (CurrentState == DocumentState.Draft && nextState == DocumentState.Revisi)
                CurrentState = nextState;
            else if (CurrentState == DocumentState.Revisi && nextState == DocumentState.SiapKumpul)
                CurrentState = nextState;
            else if (CurrentState == DocumentState.SiapKumpul && nextState == DocumentState.Revisi)
                CurrentState = nextState;
            else if (CurrentState == DocumentState.Revisi && nextState == DocumentState.Draft)
                CurrentState = nextState;
            else
                throw new InvalidOperationException($"[Automata Error] Transisi dokumen dari {CurrentState} ke {nextState} tidak diizinkan.");
        }
    }

    public static class ProjectManager
    {
        public static List<UnifiedTask> Tasks { get; private set; } = new List<UnifiedTask>();
        public static List<TeamMember> Members { get; private set; } = new List<TeamMember>();

        public static int StartMonth { get; private set; } = 1;
        public static bool IsInitialized { get; private set; } = false;

        public static DocumentAutomata DocAutomata { get; } = new DocumentAutomata();

        private static readonly Dictionary<string, (int weight, int dayMultiplier)> WBSConfig =
            new Dictionary<string, (int, int)>
        {
            { "UI", (3, 2) },
            { "BACKEND", (8, 3) },
            { "DATABASE", (7, 2) },
            { "DOKUMENTASI", (2, 3) },
            { "TESTING", (5, 2) }
        };

        public const int MaxWeightPerMonth = 40;

        private static readonly string[] MonthNames = {
            "Januari", "Februari", "Maret", "April", "Mei", "Juni",
            "Juli", "Agustus", "September", "Oktober", "November", "Desember"
        };

        public static string GetMonthName(int relativeMonth)
        {
            int targetIndex = ((StartMonth - 1) + (relativeMonth - 1)) % 12;
            return MonthNames[targetIndex];
        }

        public static void InitializeProject(int startMonth)
        {
            StartMonth = startMonth;
            IsInitialized = true;
            Tasks.Clear();
            LoadTeamMembers();
        }

        public static void LoadTeamMembers()
        {
            try
            {
                string[] possiblePaths = { "team.json", "../team.json", "../../team.json", "/Users/gentahalilintar/Documents/KPL/tubes-hub/team.json" };
                string filePath = possiblePaths.FirstOrDefault(p => File.Exists(p)) ?? "";
                
                if (!string.IsNullOrEmpty(filePath))
                {
                    string jsonString = File.ReadAllText(filePath);
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var members = JsonSerializer.Deserialize<List<TeamMember>>(jsonString, options);
                    if (members != null)
                    {
                        Members = members;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gagal memuat tim: {ex.Message}");
            }
        }

        public static void AddTaskWBS(string category, string title, string detail)
        {
            if (!WBSConfig.ContainsKey(category))
                throw new ArgumentException("Kategori tidak valid.");

            var config = WBSConfig[category];
            int relMonth = 1;
            bool scheduled = false;

            while (!scheduled)
            {
                int currentMonthWeight = Tasks.Where(t => t.RelativeMonth == relMonth).Sum(t => t.Weight);

                if (currentMonthWeight + config.weight <= MaxWeightPerMonth)
                {
                    Tasks.Add(new UnifiedTask
                    {
                        Category = category,
                        Title = title,
                        Detail = detail,
                        Weight = config.weight,
                        BobotLevel = config.weight > 5 ? "Berat" : "Ringan",
                        EstimatedDays = config.weight * config.dayMultiplier,
                        RelativeMonth = relMonth
                    });
                    scheduled = true;
                }
                else
                {
                    relMonth++;
                }
            }
        }

        public static void SaveReport()
        {
            try
            {
                var report = new
                {
                    ProjectStartMonth = StartMonth,
                    TotalTasks = Tasks.Count,
                    TeamMembers = Members,
                    Tasks = Tasks
                };

                string json = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText("laporan_akhir.json", json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gagal menyimpan laporan: {ex.Message}");
            }
        }
    }
}
