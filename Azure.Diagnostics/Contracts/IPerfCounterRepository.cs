using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Diagnostics.Models;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.Diagnostics.Management;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace Azure.Diagnostics.Contracts
{
    public interface IPerfCounterRepository
    {
        Task<IEnumerable<PerfCounterEntity>> GetPerfCounters(IEnumerable<PerfCounter> perfcounters, DateTime from,
            DateTime to);

    }
}
