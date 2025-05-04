
using Core.Interfaces;
using Core.Models;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Infrastructure.Plugins
{
    public class FileLoggerPlugin : IMonitorPlugin
    {
        private readonly string _logFilePath;

        public FileLoggerPlugin()
        {
            // Get the directory where the application is running
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Create a logs folder in the project directory
            var logDirectory = Path.Combine(baseDirectory, "Logs");

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            _logFilePath = Path.Combine(logDirectory, "metrics_log.txt");
        }

        public async Task ExecuteAsync(SystemMetrics metrics)
        {
            var logEntry = BuildLogEntry(metrics);
            await File.AppendAllTextAsync(_logFilePath, logEntry);
        }

        private string BuildLogEntry(SystemMetrics metrics)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"[{DateTime.Now}]");
            sb.AppendLine($"CPU Usage: {metrics.CpuUsagePercent:F2}%");
            sb.AppendLine($"Memory: {metrics.Memory.UsedMB:F2}MB / {metrics.Memory.TotalMB:F2}MB");
            sb.AppendLine($"Disk: {metrics.Disk.UsedMB:F2}MB / {metrics.Disk.TotalMB:F2}MB");
            sb.AppendLine(new string('-', 40));
            return sb.ToString();
        }
    }
}