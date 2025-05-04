using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models;
using Newtonsoft.Json;

namespace Infrastructure.Services.Plugins
{
    public class SlackAlertPlugin : IMonitorPlugin
    {
        private readonly string _slackWebhookUrl;

        public SlackAlertPlugin(string slackWebhookUrl)
        {
            _slackWebhookUrl = slackWebhookUrl;
        }

        public async Task ExecuteAsync(SystemMetrics metrics)
        {
            // Define threshold values for alerts
            const float cpuThreshold = 80.0f;
            const float memoryThreshold = 80.0f; // Percentage used

            // Check if the CPU or Memory exceeds the threshold
            if (metrics.CpuUsagePercent > cpuThreshold || (metrics.Memory.UsedMB / metrics.Memory.TotalMB) * 100 > memoryThreshold)
            {
                var message = new
                {
                    text = $"Alert: System metrics exceed thresholds!\n" +
                           $"CPU Usage: {metrics.CpuUsagePercent}%\n" +
                           $"Memory Usage: {(metrics.Memory.UsedMB / metrics.Memory.TotalMB) * 100}%"
                };

                var jsonMessage = JsonConvert.SerializeObject(message);
                await SendMessageToSlack(jsonMessage);
            }
        }

        private async Task SendMessageToSlack(string jsonMessage)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(_slackWebhookUrl, content);
                response.EnsureSuccessStatusCode(); // Throw if response is not successful
            }
        }
    }
}
