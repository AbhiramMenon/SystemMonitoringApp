using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Events;
using Core.Models;

namespace Core.Interfaces
{
    public interface IMonitoringService
    {
        event EventHandler<MetricsUpdatedEventArgs>? MetricsUpdated;
        Task StartAsync();
        void Stop();
    }
}
