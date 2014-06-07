using System.Configuration;
using System.Threading.Tasks;
using Azure.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Azure.Diagnostics.Models;
using Azure.Diagnostics.Website.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Diagnostics.Website.Controllers
{
    public class PerfCountersController : ApiController
    {
        public static long ToJavascriptTimestamp(DateTime input)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var time = input.Subtract(new TimeSpan(epoch.Ticks));
            return (long)(time.Ticks / 10000);
        }
        private readonly IPerfCounterRepository repository;
        private readonly IDeploymentRepository deployments;

        public PerfCountersController()
        {
            var config = ConfigurationManager.AppSettings["Connection"];
            this.repository = new PerfCounterRepository(config);
            this.deployments = new DeploymentRepository(config);
        }

        public async Task<IEnumerable<PerfCounterReponse>> Get(int id, string deployId, [FromUri]string[] counters, [FromUri]DateTime endDateUtc, string interval)
        {
            var deployment = await this.deployments.GetDeployment(id);
            var request = counters.Select(t => new PerfCounter
            {
                CounterName = t,
                RoleName = deployment.RoleName,
                DeploymentId = deployId
            });

            var timeInterval = interval.ToLower().Equals("day") ? 60*6 : 60;

            var perfCounters = await repository.GetPerfCounters(request, endDateUtc.AddMinutes(timeInterval * -1), endDateUtc);

            var results = perfCounters.OrderBy(t => t.EventTickCount).GroupBy(t => t.CounterName, t => t, (name, counter) => new 
            {
                CounterName = name,
                Values = counter.Select(t => new 
                {
                    Ticks = new DateTime(t.EventDateTime.Year, t.EventDateTime.Month, t.EventDateTime.Day, t.EventDateTime.Hour, t.EventDateTime.Minute, 0),
                    Value = t.CounterValue
                })
            }).Select(t => new PerfCounterReponse
            {
                CounterName = t.CounterName,
                Values = t.Values.GroupBy(p => p.Ticks, p => p.Value, (ticks, values) => new PerfCounterEntry
                {
                    Ticks = ToJavascriptTimestamp(ticks),
                    Value = values.Average()
                })
            });

            return results;
        }
    }
}