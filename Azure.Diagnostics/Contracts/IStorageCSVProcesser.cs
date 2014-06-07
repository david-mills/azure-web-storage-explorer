using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Diagnostics.Contracts
{
    public interface IStorageCSVProcesser
    {
        Task<string> GetCSV(IList<DynamicTableEntity> results, string[] headings, bool addHeaderRow);
    }
}
