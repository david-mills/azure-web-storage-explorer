using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Diagnostics.Contracts;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Diagnostics
{
    public class StorageRepository : IStorageRepository
    {
        private CloudStorageAccount account;
        private CloudTableClient tableClient;

        public StorageRepository(string connectionString)
        {
            this.account = CloudStorageAccount.Parse(connectionString);
            this.tableClient = account.CreateCloudTableClient();
        }
        
        public async Task<IEnumerable<string>> GetTables()
        {
            return await Task.FromResult(tableClient.ListTables().Select(t => t.Name));
        }

        public async Task<TableQuerySegment<DynamicTableEntity>> GetTableData(string tableName, string filter, TableContinuationToken continuationToken)
        {
            return await GetData(tableName, filter, continuationToken);
        }

        public async Task<DynamicTableEntity> GetFirstRowTableData(string tableName)
        {
            var table = tableClient.GetTableReference(tableName);
            if (!table.Exists())
            {
                throw new ArgumentException("Table does not exist", tableName);
            }

            var query = await table.ExecuteQuerySegmentedAsync(new TableQuery<DynamicTableEntity>().Take(1), null);

            return query.Results.FirstOrDefault();
        }

        public async Task<TableQuerySegment<DynamicTableEntity>> GetTableDataByDate(string tableName, string filter, DateTime from, DateTime to, TableContinuationToken continuationToken, bool hasLeadingZero)
        {
            var fromUtc = from.ToUniversalTime();
            var toUtc = to.ToUniversalTime();
            var lead = hasLeadingZero ? "0" : "";
            string fromFilter = TableQuery.GenerateFilterCondition("PartitionKey",
                QueryComparisons.GreaterThanOrEqual, string.Format("{0}{1}", lead, fromUtc.Ticks));
            string toFilter = TableQuery.GenerateFilterCondition("PartitionKey",
                QueryComparisons.LessThan, string.Format("{0}{1}", lead, toUtc.Ticks));
            var filterQuery = TableQuery.CombineFilters(toFilter, TableOperators.And, fromFilter);
            if (!String.IsNullOrEmpty(filter))
            {
                filterQuery = TableQuery.CombineFilters(filter, TableOperators.And, filterQuery);
            }
            return await GetData(tableName, filterQuery, continuationToken);
        }

        private async Task<TableQuerySegment<DynamicTableEntity>> GetData(string tableName, string filter, TableContinuationToken continuationToken)
        {
            var table = tableClient.GetTableReference(tableName);
            if (!table.Exists())
            {
                throw new ArgumentException("Table does not exist", tableName);
            }

            var query = new TableQuery<DynamicTableEntity>();
            if (!String.IsNullOrEmpty(filter))
            {
                query = query.Where(filter);
            }

            if (continuationToken != null)
            {
                continuationToken.NextTableName = tableName;
            }
            return await table.ExecuteQuerySegmentedAsync(query, continuationToken);
        }
    }
}

