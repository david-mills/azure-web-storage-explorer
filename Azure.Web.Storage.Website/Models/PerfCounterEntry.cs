using System;

namespace Azure.Web.Storage.Website.Models
{
    public class PerfCounterEntry
    {
        public double Value { get; set; }
        public long Ticks { get; set; }
    }
}