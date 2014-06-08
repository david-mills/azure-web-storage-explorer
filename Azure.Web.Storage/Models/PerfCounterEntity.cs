using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Web.Storage.Models
{
    public class PerfCounterEntity : TableEntity
    {
        public long EventTickCount { get; set; }

        public string RoleInstance { get; set; }

        public string CounterName { get; set; }

        public double CounterValue { get; set; }

        public DateTime EventDateTime
        {
            get { return new DateTime(EventTickCount); }
        }
    }
}
