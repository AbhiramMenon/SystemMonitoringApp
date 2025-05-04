using Xunit;
using Moq;
using Core.Models;
using Core.Interfaces;
using Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Tests
{
    public class MonitoringServiceTests
    {
        [Fact]
        public async Task MonitorOnceAsync_CallsPluginsWithMetrics()
        {
            // Arrange
            var dummyMetrics = new SystemMetrics
            {
                CpuUsagePercent = 25.5f,
                Memory = new MemoryMetrics { TotalMB = 8000, UsedMB = 4000 },
                Disk = new DiskMetrics {TotalMB = 512, UsedMB = 128 }
            };

            var metricsProviderMock = new Mock<ISystemMetricsProvider>();
            metricsProviderMock
                .Setup(p => p.GetSystemMetricsAsync())
                .ReturnsAsync(dummyMetrics);

            var pluginMock = new Mock<IMonitorPlugin>();
            var plugins = new List<IMonitorPlugin> { pluginMock.Object };

            var service = new MonitoringService(metricsProviderMock.Object, plugins, 1);

            // Act
            await service.MonitorOnceAsync();

            // Assert
            pluginMock.Verify(p => p.ExecuteAsync(It.Is<SystemMetrics>(m =>
                m.CpuUsagePercent == dummyMetrics.CpuUsagePercent &&
                m.Memory.TotalMB == dummyMetrics.Memory.TotalMB &&
                m.Memory.UsedMB == dummyMetrics.Memory.UsedMB &&
                m.Disk.TotalMB == dummyMetrics.Disk.TotalMB &&
                m.Disk.TotalMB == dummyMetrics.Disk.TotalMB
            )), Times.Once);
        }
    }
}
