using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Web.Storage.Models;

namespace Azure.Web.Storage.Contracts
{
    public interface IPerfCounterRepository
    {
        Task<IEnumerable<PerfCounterEntity>> GetPerfCounters(IEnumerable<PerfCounter> perfcounters, DateTime from,
            DateTime to);

    }
}
