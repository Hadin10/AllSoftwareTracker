using System;
using System.Diagnostics;
using System.Net.Http;

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
        string existingFileHIP = "";
        string existingFilePIP = "";
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (Directory.Exists(@"D:\"))
            {
                fileName = @"D:\" + "AllTracker_LogFile.txt";
                existingFileHIP = @"D:\" + "HIP_LogFile.txt";
                existingFilePIP = @"D:\" + "HIP_LogFile.txt";
            }
            else if (Directory.Exists(@"E:\"))
            {
                fileName = @"E:\" + "AllTracker_LogFile.txt";
                existingFileHIP = @"E:\" + "HIP_LogFile.txt";
                existingFilePIP = @"E:\" + "HIP_LogFile.txt";
            }
            else
            {
                fileName = Path.Combine(Directory.GetCurrentDirectory(), "AllTracker_LogFile.txt");
                existingFileHIP = Path.Combine(Directory.GetCurrentDirectory(), "HIP_LogFile.txt");
                existingFilePIP = Path.Combine(Directory.GetCurrentDirectory(), "HIP_LogFile.txt");
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
                string processName1 = "SmartManufacturingLocal_PIP_RIP_DIP_PLC";
                var processes1 = Process.GetProcessesByName(processName1);

                string processName2 = "SmartManufacturingLocal_HIP_PLC";
                var processes2 = Process.GetProcessesByName(processName2);
                if (processes1.Length == 0)
                {
                    File.AppendAllText(fileName, "\nProcess not running. " + DateTime.Now.ToString());
                    await SendWhatsAppViaCallMeBot("SmartManufacturingLocal_PIP_RIP_DIP_PLC is not running!");
                }
                else
                {
                    foreach (var process in processes1)
                    {
                        if (!process.Responding)
                        {
                            File.AppendAllText(fileName, "\nProcess not responding. " + DateTime.Now.ToString());
                            await SendWhatsAppViaCallMeBot("SmartManufacturingLocal_PIP_RIP_DIP_PLC is not running!");
                        }
                        else
                        {
                            File.AppendAllText(fileName, "\nProcess is running fine. " + DateTime.Now.ToString());
                        }
                    }
                }

                if (processes2.Length == 0)
                {
                    File.AppendAllText(fileName, "\nProcess not running. " + DateTime.Now.ToString());
                    await SendWhatsAppViaCallMeBot("SmartManufacturingLocal_HIP_PLC is not running!");
                }
                else
                {
                    foreach (var process in processes2)
                    {
                        if (!process.Responding)
                        {
                            File.AppendAllText(fileName, "\nProcess not responding. " + DateTime.Now.ToString());
                            await SendWhatsAppViaCallMeBot("SmartManufacturingLocal_HIP_PLC is not running!");
                        }
                        else
                        {
                            File.AppendAllText(fileName, "\nSmartManufacturingLocal_HIP_PLC is running fine. " + DateTime.Now.ToString());
                        }
                    }
                }


                #region FOR HIP
                if (File.Exists(existingFileHIP))
                {
                    string? lastLine = "";
                    lastLine = File.ReadLines(existingFileHIP).LastOrDefault();
                    if (!string.IsNullOrWhiteSpace(lastLine) && DateTime.TryParse(lastLine, out DateTime recordedDate))
                    {
                        if ((DateTime.Now - recordedDate).TotalMinutes > 10)
                        {
                            await SendWhatsAppViaCallMeBot("SmartManufacturingLocal_HIP is not running!");
                        }
                    }
                    else
                    {
                        await SendWhatsAppViaCallMeBot("SmartManufacturingLocal_HIP is under exception!");
                    }
                }
                else
                {
                    await SendWhatsAppViaCallMeBot("SmartManufacturingLocal_HIP's file does not find!");
                }
                #endregion

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

                await Task.Delay(1800000, stoppingToken);
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
    }
}
