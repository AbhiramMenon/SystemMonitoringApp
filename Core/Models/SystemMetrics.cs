using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class SystemMetrics
    {
        public float CpuUsagePercent { get; set; }
        public MemoryMetrics Memory { get; set; } = new();
        public DiskMetrics Disk { get; set; } = new();
    }
}
