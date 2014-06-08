using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Web.Storage.Contracts;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Web.Storage
{
    public class TokenSerializer : ITokenSerializer
    {
        public TableContinuationToken DeSerialize(string continuationToken)
        {
            if (String.IsNullOrEmpty(continuationToken))
            {
                return null;
            }
            var parts = continuationToken.Split('|');
            if (parts.Length != 2)
            {
                return null;
            }
            return new TableContinuationToken
            {
                NextPartitionKey = parts[0],
                NextRowKey = parts[1]
            };
        }

        public string Serialize(TableContinuationToken continuationToken)
        {
            if (continuationToken == null)
            {
                return string.Empty;
            }
            return string.Format("{0}|{1}", continuationToken.NextPartitionKey, continuationToken.NextRowKey);
        }
    }
}




