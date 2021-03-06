﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Web.Storage.Contracts;
using Azure.Web.Storage.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Web.Storage
{
    public class PerfCounterRepository : IPerfCounterRepository
    {
        private CloudStorageAccount account;
        private CloudTableClient tableClient;
        private readonly CloudTable table;

        public PerfCounterRepository(string connectionString)
        {
            this.account = CloudStorageAccount.Parse(connectionString);
            this.tableClient = account.CreateCloudTableClient();
            this.table = tableClient.GetTableReference("WADPerformanceCountersTable");
        }

        public async Task<IEnumerable<PerfCounterEntity>> GetPerfCounters(IEnumerable<PerfCounter> perfcounters, DateTime from, DateTime to)
        {
            string timeQuery = TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("PartitionKey",
                QueryComparisons.GreaterThanOrEqual, string.Format("0{0}", from.ToUniversalTime().Ticks)), TableOperators.And, TableQuery.GenerateFilterCondition("PartitionKey",
                QueryComparisons.LessThan, string.Format("0{0}", to.ToUniversalTime().Ticks)));
            string perfCounterQuery = "";
            foreach (var counter in perfcounters)
            {
                var combinedQuery = TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("DeploymentId",
                    QueryComparisons.Equal, counter.DeploymentId.Trim().ToLower()), TableOperators.And,
                    TableQuery.GenerateFilterCondition("Role",
                    QueryComparisons.GreaterThanOrEqual, counter.RoleName));

                var counterQuery = TableQuery.CombineFilters(combinedQuery, TableOperators.And,TableQuery.GenerateFilterCondition("CounterName",
                    QueryComparisons.Equal, counter.CounterName));

                if (!String.IsNullOrEmpty(counter.RoleInstance))
                {
                    counterQuery = TableQuery.CombineFilters(counterQuery, TableOperators.And,
                    TableQuery.GenerateFilterCondition("RoleInstance",
                        QueryComparisons.Equal, counter.RoleInstance));
                }
                if (String.IsNullOrEmpty(perfCounterQuery))
                {
                    perfCounterQuery = counterQuery;
                }
                else
                {
                    perfCounterQuery = TableQuery.CombineFilters(perfCounterQuery, TableOperators.Or,
                       counterQuery);
                }
                
            }

            var fullQuery = TableQuery.CombineFilters(timeQuery, TableOperators.And,
                    perfCounterQuery);
            var query = new TableQuery<PerfCounterEntity>().Where(fullQuery);
            var results = await table.ExecuteQuerySegmentedAsync(query, null);
            var token = results.ContinuationToken;
            while (token != null)
            {
                var newResult = await table.ExecuteQuerySegmentedAsync(query, token);
                results.Results.AddRange(newResult.Results);
                token = newResult.ContinuationToken;

            }

            return results;

        }
    }
}
