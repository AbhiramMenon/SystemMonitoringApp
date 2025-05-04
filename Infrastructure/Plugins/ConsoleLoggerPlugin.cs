using Core.Interfaces;
using Core.Models;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Plugins
{
    public class ConsoleLoggerPlugin : IMonitorPlugin
    {
        public Task ExecuteAsync(SystemMetrics metrics)
        {
            Console.Clear();
            Console.WriteLine($"==== System Metrics - {DateTime.Now} ====");
            Console.WriteLine($"CPU Usage: {metrics.CpuUsagePercent:F2}%");
            Console.WriteLine($"Memory: {metrics.Memory.UsedMB:F2} MB / {metrics.Memory.TotalMB:F2} MB ({(metrics.Memory.UsedMB / metrics.Memory.TotalMB * 100):F2}%)");
            Console.WriteLine($"Disk: {metrics.Disk.UsedMB:F2} MB / {metrics.Disk.TotalMB:F2} MB ({(metrics.Disk.UsedMB / metrics.Disk.TotalMB * 100):F2}%)");
            Console.WriteLine("====================================");
            return Task.CompletedTask;
        }
    }
}
