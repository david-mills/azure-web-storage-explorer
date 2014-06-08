using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace Azure.Web.Storage.Website.Models
{
    public class StorageEntry
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public IEnumerable<string> Values { get; set; }
    }
}