using System;
using System.Diagnostics;
using System.Net.Http;
using Microsoft.Data.SqlClient;
namespace AllSoftwareTracker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }
        string fileName = "";
        string existingFilePIP = "";
        string connectionString = @"Server=172.17.2.117;User Id=sa;Password=aaaaAAAA0000;Database=SmartManufacturingV2;TrustServerCertificate=True;MultipleActiveResultSets=True;";
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (Directory.Exists(@"D:\"))
            {
                fileName = @"D:\" + "AllTracker_LogFile.txt";
                existingFilePIP = @"D:\" + "PIP_LogFile.txt";
            }
            else if (Directory.Exists(@"E:\"))
            {
                fileName = @"E:\" + "AllTracker_LogFile.txt";
                existingFilePIP = @"E:\" + "PIP_LogFile.txt";
            }
            else
            {
                fileName = Path.Combine(Directory.GetCurrentDirectory(), "AllTracker_LogFile.txt");
                existingFilePIP = Path.Combine(Directory.GetCurrentDirectory(), "PIP_LogFile.txt");
            }
            if (!File.Exists(fileName))
            {
                using (FileStream fs = File.Create(fileName))
                {

                }
            }
            File.AppendAllText(fileName, "Start at " + DateTime.Now.ToString() + "\n");
            while (!stoppingToken.IsCancellationRequested)
            {
                #region FOR PIP
                if (File.Exists(existingFilePIP))
                {
                    string? lastLine = "";
                    lastLine = File.ReadLines(existingFilePIP).LastOrDefault();
                    if (!string.IsNullOrWhiteSpace(lastLine) && DateTime.TryParse(lastLine, out DateTime recordedDate))
                    {
                        if ((DateTime.Now - recordedDate).TotalMinutes > 10)
                        {
                            await SendWhatsAppViaCallMeBot("SmartManufacturingLocal_PIP is not running!");
                        }
                    }
                    else
                    {
                        await SendWhatsAppViaCallMeBot("SmartManufacturingLocal_PIP is under exception!");
                    }
                }
                else
                {
                    await SendWhatsAppViaCallMeBot("SmartManufacturingLocal_PIP's file does not find!");
                }
                #endregion
                var existingUsers = await LoadUsersFromDatabaseAsync();
                await Task.Delay(180, stoppingToken);
            }
        }

        public async Task SendWhatsAppViaCallMeBot(string message)
        {
            using (var client = new HttpClient())
            {
                string phone = "8801704151143";
                string apiKey = "1552297";
                string url = $"https://api.callmebot.com/whatsapp.php?phone={phone}&text={Uri.EscapeDataString(message)}&apikey={apiKey}";

                var response = await client.GetAsync(url);
                string result = await response.Content.ReadAsStringAsync();
                File.AppendAllText(fileName, "\nWhatsApp API response: " + result);
            }
        }
        private async Task<decimal> LoadUsersFromDatabaseAsync()
        {
            string query = "SELECT Id, StaffId, Name FROM Users where IsActive=1 and IsDelete=0";

            await using (SqlConnection conn = new SqlConnection(connectionString))
            await using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                await conn.OpenAsync();
                await using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var user = new
                        {
                            Id = reader.GetInt64(reader.GetOrdinal("Id")),
                            StaffId = reader.GetString(reader.GetOrdinal("StaffId")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };
                    }
                }
            }
            return 0;
        }
    }
}
