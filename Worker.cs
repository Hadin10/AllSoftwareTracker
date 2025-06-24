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
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (Directory.Exists(@"D:\"))
            {
                fileName = @"D:\" + "allTracker.txt";
            }
            else if (Directory.Exists(@"E:\"))
            {
                fileName = @"E:\" + "allTracker.txt";
            }
            else
            {
                fileName = Path.Combine(Directory.GetCurrentDirectory(), "allTracker.txt");
            }
            if (!File.Exists(fileName))
            {
                using (FileStream fs = File.Create(fileName))
                {

                }
            }
            File.AppendAllText(fileName, "\nStart at " + DateTime.Now.ToString());
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
