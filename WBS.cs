using System;
using System.Collections.Generic;
using System.Linq;

namespace tubes_hub.Tubes_Hub_KPL_
{
    //Generics
    public class TaskItem<T>
    {
        public string Category { get; set; }
        public string Title { get; set; } 
        public T Detail { get; set; }
        public int Weight { get; set; }
        public int EstimatedDays { get; set; } 
        public int RelativeMonth { get; set; }

        public TaskItem(string category, string title, T detail, int weight, int days, int relMonth)
        {
            Category = category;
            Title = title;
            Detail = detail;
            Weight = weight;
            EstimatedDays = days;
            RelativeMonth = relMonth;
        }
    }

    public class WBSModule
    {
        //table Driven
        private static readonly Dictionary<string, (int weight, int dayMultiplier)> WBSConfig =
            new Dictionary<string, (int, int)>
        {
            { "UI", (3, 2) },          
            { "BACKEND", (8, 3) },     
            { "DATABASE", (7, 2) },
            { "DOKUMENTASI", (2, 3) },
            { "TESTING", (5, 2) }
        };

        private readonly string[] MonthNames = {
            "Januari", "Februari", "Maret", "April", "Mei", "Juni",
            "Juli", "Agustus", "September", "Oktober", "November", "Desember"
        };

        private List<TaskItem<string>> projectTasks = new List<TaskItem<string>>();
        private const int MAX_WEIGHT_PER_MONTH = 40;
        private int _startMonthIndex;

        public WBSModule(int startMonth) { _startMonthIndex = startMonth - 1; }

        private string GetMonthName(int relativeMonth)
        {
            int targetIndex = (_startMonthIndex + (relativeMonth - 1)) % 12;
            return MonthNames[targetIndex];
        }
        public void AddTask(string category, string title, string detail)
        {
            var config = WBSConfig[category];
            int relMonth = 1;
            bool scheduled = false;

            while (!scheduled)
            {
                int currentMonthWeight = projectTasks.Where(t => t.RelativeMonth == relMonth).Sum(t => t.Weight);

                if (currentMonthWeight + config.weight <= MAX_WEIGHT_PER_MONTH)
                {
                    projectTasks.Add(new TaskItem<string>(
                        category, title, detail, config.weight, config.weight * config.dayMultiplier, relMonth
                    ));
                    Console.WriteLine($"\n[BERHASIL] Masuk perencanaan bulan: {GetMonthName(relMonth)}");
                    scheduled = true;
                }
                else
                {
                    relMonth++;
                }
            }
        }

        public void ShowWBSPlan()
        {
            if (!projectTasks.Any())
            {
                Console.WriteLine("\nBelum ada tugas yang direncanakan.");
                return;
            }

            var groupedTasks = projectTasks.OrderBy(t => t.RelativeMonth).GroupBy(t => t.RelativeMonth);

            Console.WriteLine("\n====================================================================================");
            Console.WriteLine("                            TIMELINE PERENCANAAN PROYEK                             ");
            Console.WriteLine("====================================================================================");

            foreach (var group in groupedTasks)
            {
                Console.WriteLine($"\n{GetMonthName(group.Key).ToUpper()} (Beban: {group.Sum(t => t.Weight)}/40)");
                Console.WriteLine("------------------------------------------------------------------------------------");
                foreach (var task in group)
                {
                    Console.WriteLine($"- {task.Category.PadRight(12)} | {task.Title.PadRight(20)} | {task.Detail.PadRight(30)} | {task.EstimatedDays} Hari");
                }
            }
            Console.WriteLine("====================================================================================");
        }
    }
}