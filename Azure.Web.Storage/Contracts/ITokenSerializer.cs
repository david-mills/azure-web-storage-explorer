using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Web.Storage.Contracts
{
    public interface ITokenSerializer
    {
        TableContinuationToken DeSerialize(string continuationToken);

        string Serialize(TableContinuationToken continuationToken);
    }
}




