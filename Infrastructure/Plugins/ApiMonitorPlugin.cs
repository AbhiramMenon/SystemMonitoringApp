using Core.Interfaces;
using Core.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class ApiMonitorPlugin : IMonitorPlugin
{
    private readonly HttpClient _httpClient;
    private readonly string _endpointUrl;

    public ApiMonitorPlugin(HttpClient httpClient, string endpointUrl)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _endpointUrl = endpointUrl ?? throw new ArgumentNullException(nameof(endpointUrl));
    }

    public async Task ExecuteAsync(SystemMetrics metrics)
    {
        var payload = new
        {
            CpuUsagePercent = metrics.CpuUsagePercent,
            Memory = new { UsedMB = metrics.Memory.UsedMB, TotalMB = metrics.Memory.TotalMB },
            Disk = new { UsedMB = metrics.Disk.UsedMB, TotalMB = metrics.Disk.TotalMB }
        };

        var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json");
        try
        {
            var response = await _httpClient.PostAsync(_endpointUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[API] Failed to POST: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ApiPlugin] Failed to post data: {ex.Message}");
        }
    }
}
