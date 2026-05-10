using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace TubesHub.ModulProgress
{
    public class Holiday
    {
        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("localName")]
        public string? LocalName { get; set; } 
    }

    public class HolidayChecker
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<bool> IsHolidayAsync(DateTime deadline)
        {
            if (deadline.Year < 2000 || deadline.Year > 2100)
            {
                throw new ArgumentOutOfRangeException(nameof(deadline), "Error: Tahun deadline di luar jangkauan pengecekan.");
            }

            string url = $"https://date.nager.at/api/v3/PublicHolidays/{deadline.Year}/ID";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                
                var holidays = JsonSerializer.Deserialize<List<Holiday>>(jsonResponse);

                string formattedDeadline = deadline.ToString("yyyy-MM-dd");
                
                var holidayMatch = holidays?.FirstOrDefault(h => h.Date == formattedDeadline);

                if (holidayMatch != null)
                {
                    Console.WriteLine($"[Peringatan API] Hati-hati! Tanggal {deadline.ToString("dd MMM yyyy")} adalah hari libur: {holidayMatch.LocalName}");
                    return true;
                }

                Console.WriteLine($"[Aman] Tanggal {deadline.ToString("dd MMM yyyy")} bukan hari libur nasional.");
                return false;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"[Error API] Gagal menghubungi server kalender: {e.Message}");
                return false; 
            }
        }
    }
}