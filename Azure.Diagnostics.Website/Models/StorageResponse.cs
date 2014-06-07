using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Azure.Diagnostics.Website.Models
{
    public class StorageResponse
    {
        public string ContinuationToken { get; set; }
        public IEnumerable<string> Headings { get; set; }
        public IEnumerable<StorageEntry> StorageEntries { get; set; }
    }
}