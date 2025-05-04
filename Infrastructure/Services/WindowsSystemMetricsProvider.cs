using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models;
using System.Diagnostics;
using System.IO;

namespace Infrastructure.Services
{
    namespace Infrastructure.Services
    {
        public class WindowsSystemMetricsProvider : ISystemMetricsProvider
        {
            private readonly PerformanceCounter _cpuCounter;
            private readonly PerformanceCounter _ramAvailableCounter;
            private readonly long _estimatedTotalRamMB;

            public WindowsSystemMetricsProvider()
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _ramAvailableCounter = new PerformanceCounter("Memory", "Available MBytes");

                // Estimate total RAM by looking at the largest value from various sources
                _estimatedTotalRamMB = EstimateTotalRamMB();

                // Prime the CPU counter (first call returns 0)
                _cpuCounter.NextValue();
                Thread.Sleep(500);
            }

            public async Task<SystemMetrics> GetSystemMetricsAsync()
            {
                return await Task.Run(() => {
                    var cpuUsage = _cpuCounter.NextValue();
                    var availableRamMB = _ramAvailableCounter.NextValue();

                    // Calculate used RAM
                    var usedRamMB = _estimatedTotalRamMB - availableRamMB;
                    if (usedRamMB < 0) usedRamMB = 0; // Guard against negative values

                    var memory = new MemoryMetrics
                    {
                        TotalMB = _estimatedTotalRamMB,
                        UsedMB = usedRamMB
                    };

                    var disk = GetPrimaryDiskUsage();

                    return new SystemMetrics
                    {
                        CpuUsagePercent = cpuUsage,
                        Memory = memory,
                        Disk = disk
                    };
                });
            }

            private long EstimateTotalRamMB()
            {
                try
                {
                    // Try to get a reasonable estimate from the system drive's total size
                    // Most modern systems have RAM that's about 1/8 to 1/4 of their primary drive size
                    var primaryDrive = DriveInfo.GetDrives()
                        .FirstOrDefault(d => d.IsReady && d.Name == Path.GetPathRoot(Environment.SystemDirectory));

                    if (primaryDrive != null)
                    {
                        // Estimate RAM as 1/8 of drive size, capped at 64GB
                        long estimatedFromDrive = Math.Min(
                            primaryDrive.TotalSize / (1024L * 1024L * 8),
                            64 * 1024); // 64GB cap

                        // Use at least 4GB 
                        return Math.Max(estimatedFromDrive, 4 * 1024);
                    }
                }
                catch
                {
                    // Silently fail and use default
                }

                // Fallback to a reasonable default for modern systems
                return 16 * 1024; // 16GB
            }

            private DiskMetrics GetPrimaryDiskUsage()
            {
                try
                {
                    var drive = DriveInfo.GetDrives()
                        .FirstOrDefault(d => d.IsReady && d.Name == Path.GetPathRoot(Environment.SystemDirectory));

                    if (drive != null)
                    {
                        var totalMB = drive.TotalSize / (1024f * 1024f);
                        var usedMB = (drive.TotalSize - drive.TotalFreeSpace) / (1024f * 1024f);
                        return new DiskMetrics
                        {
                            TotalMB = totalMB,
                            UsedMB = usedMB
                        };
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting disk metrics: {ex.Message}");
                }

                // Fallback with reasonable values
                return new DiskMetrics
                {
                    TotalMB = 500 * 1024, // 500GB
                    UsedMB = 250 * 1024   // 250GB (50% usage)
                };
            }
        }
    }
}