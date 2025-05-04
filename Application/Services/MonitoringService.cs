using Core.Events;
using Core.Interfaces;
using Core.Models;

namespace Application.Services
{
    public class MonitoringService : IMonitoringService
    {
        private readonly ISystemMetricsProvider _metricsProvider;
        private readonly IEnumerable<IMonitorPlugin> _plugins;
        private readonly int _intervalSeconds;
        private CancellationTokenSource _cts;

        public event EventHandler<MetricsUpdatedEventArgs>? MetricsUpdated;

        public MonitoringService(
            ISystemMetricsProvider metricsProvider,
            IEnumerable<IMonitorPlugin> plugins,
            int intervalSeconds = 5)
        {
            _metricsProvider = metricsProvider;
            _plugins = plugins;
            _intervalSeconds = intervalSeconds;
            _cts = new CancellationTokenSource();
        }

        public async Task StartAsync()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                await MonitorOnceAsync();

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(_intervalSeconds), _cts.Token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        public async Task MonitorOnceAsync()
        {
            try
            {
                // 1. Fetch system metrics
                var metrics = await _metricsProvider.GetSystemMetricsAsync();

                // 2. Raise event to any subscribers (e.g., UI, log viewer)
                MetricsUpdated?.Invoke(this, new MetricsUpdatedEventArgs(metrics));

                // 3. Run all registered plugins
                foreach (var plugin in _plugins)
                {
                    try
                    {
                        await plugin.ExecuteAsync(metrics);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Plugin Error] {plugin.GetType().Name}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Monitoring Error] {ex.Message}");
            }
        }

        public void Stop()
        {
            _cts.Cancel();
        }
    }
}
