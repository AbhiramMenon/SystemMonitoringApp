using System;
using Core.Models;

namespace Core.Events
{
    public class MetricsUpdatedEventArgs : EventArgs
    {
        public SystemMetrics Metrics { get; }

        public MetricsUpdatedEventArgs(SystemMetrics metrics)
        {
            Metrics = metrics;
        }
    }
}
