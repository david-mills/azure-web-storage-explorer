using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Diagnostics.Contracts
{
    public interface IStorageRepository
    {
        Task<IEnumerable<string>> GetTables();

        Task<TableQuerySegment<DynamicTableEntity>> GetTableData(string tableName, string filter, TableContinuationToken continuationToken);

        Task<TableQuerySegment<DynamicTableEntity>> GetTableDataByDate(string tableName, string filter, DateTime from,
            DateTime to, TableContinuationToken continuationToken, bool hasLeadingZero);

        Task<DynamicTableEntity> GetFirstRowTableData(string tableName);

    }
}
