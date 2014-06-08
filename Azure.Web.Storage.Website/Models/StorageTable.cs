using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Azure.Web.Storage.Website.Models
{
    public class StorageTable
    {
        public string TableName { get; set; }
    }
    public class StorageTableProperties
    {
        public KeyType KeyType { get; set; }
    }
    public enum KeyType
    {
        DateTicks,
        DateTicksLeadingZero,
        String
    }
}