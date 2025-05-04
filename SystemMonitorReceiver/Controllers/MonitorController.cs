using Core.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MonitorController : ControllerBase
{
    private static SystemMetrics _latestMetrics;
    [HttpPost]
    public IActionResult Post([FromBody] SystemMetrics data)
    {
        _latestMetrics = data;
        Console.WriteLine($"Received: CPU={data.CpuUsagePercent}, RAM={data.Memory.UsedMB}, Disk={data.Disk.UsedMB}");
        return Ok();
    }

    [HttpGet]
    public IActionResult Get()
    {
        if (_latestMetrics == null)
            return NotFound("No data received yet.");

        return Ok(_latestMetrics);
    }
}
