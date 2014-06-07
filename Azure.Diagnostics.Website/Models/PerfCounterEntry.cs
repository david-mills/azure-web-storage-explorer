using System;

namespace Azure.Diagnostics.Website.Models
{
    public class PerfCounterEntry
    {
        public double Value { get; set; }
        public long Ticks { get; set; }
    }
}