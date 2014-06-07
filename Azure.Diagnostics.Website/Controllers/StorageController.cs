using System.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Azure.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Azure.Diagnostics.Website.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Diagnostics.Website.Controllers
{
    public class StorageController : ApiController
    {
        private IStorageRepository repository;
        private ITokenSerializer _tokenSerializer;
        private IStorageCSVProcesser _csvSerializer;

        public StorageController()
        {
            var config = ConfigurationManager.AppSettings["Connection"];
            this.repository = new StorageRepository(config);
            this._tokenSerializer = new TokenSerializer();
            this._csvSerializer = new StorageCSVProcesser();
        }

        public async Task<IEnumerable<StorageTable>> Get()
        {
            var tables = await this.repository.GetTables();
            return tables.Where(t => !t.StartsWith("WAD")).Select(t => new StorageTable
            {
                TableName = t
            });
        }

        public async Task<StorageTableProperties> GetProperties(string id)
        {
            var table = await this.repository.GetFirstRowTableData(id);
            if (table == null)
            {
                return null;
            }
            string partitionKey = table.PartitionKey;
            var type = KeyType.DateTicks;
            long key = 0;
            if (partitionKey.StartsWith("0"))
            {
                type = KeyType.DateTicksLeadingZero;
                partitionKey = partitionKey.Substring(1);
            }
            if (long.TryParse(partitionKey, out key))
            {
                return new StorageTableProperties
                {
                    KeyType = type
                };
            }
            return new StorageTableProperties
            {
                KeyType = KeyType.String
            };
        }

        public async Task<IEnumerable<StorageTable>> GetSystem()
        {
            var tables = await this.repository.GetTables();
            return tables.Where(t => t.StartsWith("WAD")).Select(t => new StorageTable
            {
                TableName = t
            });
        }
        public async Task<StorageResponse> Get(string id, KeyType keyType, DateTime? from = null, DateTime? to = null, string filter = "", string token = "")
        {
            var response = new StorageResponse();
            var entrys = new List<StorageEntry>();
            var contToken = _tokenSerializer.DeSerialize(token);
            TableQuerySegment<DynamicTableEntity> storageRequest = await GetData(id, keyType, contToken, from, to, filter);
            response.ContinuationToken = _tokenSerializer.Serialize(storageRequest.ContinuationToken);

            var headings = new List<string>();
            for (int i = 0; i < storageRequest.Results.Count; i++)
            {
                var item = storageRequest.Results[i];
                if (i == 0)
                {
                    foreach (var key in item.Properties.Keys)
                    {
                        headings.Add(key);
                    }
                }
                var entry = new StorageEntry
                {
                    PartitionKey = item.PartitionKey,
                    RowKey = item.RowKey,
                    TimeStamp = item.Timestamp
                };
                var values = new List<string>();
                foreach (var key in headings)
                {
                    values.Add(item.Properties.ContainsKey(key) ? GetValue(item.Properties[key]).ToString() : "");
                }
                entry.Values = values;
                entrys.Add(entry);
            }
            response.Headings = headings;
            response.StorageEntries = entrys;
            return response;
        }

        [HttpGet]
        public HttpResponseMessage Export(string id, KeyType keyType, DateTime? from = null, DateTime? to = null,
            string filter = "")
        {

            var response = Request.CreateResponse();

            response.Content = new PushStreamContent(
                async (outputStream, httpContent, transportContext) =>
                {
                    try
                    {
                        var storageRequest = await GetData(id, keyType, null, from, to, filter);
                        string[] headers = GetHeading(storageRequest.Results[0]);
                        var content = await this._csvSerializer.GetCSV(storageRequest.Results, headers, true);

                        var buffer = UTF8Encoding.UTF8.GetBytes(content);

                        await StreamContent(buffer, outputStream);

                        var contiuationToken = storageRequest.ContinuationToken;

                        while (contiuationToken != null)
                        {
                            storageRequest = await GetData(id, keyType, contiuationToken, from, to, filter);
                            content = await this._csvSerializer.GetCSV(storageRequest.Results, headers, false);
                            buffer = Encoding.UTF8.GetBytes(content);
                            await StreamContent(buffer, outputStream);
                            contiuationToken = storageRequest.ContinuationToken;
                        }
                    }
                    catch (HttpException ex)
                    {
                        if (ex.ErrorCode == -2147023667) // The remote host closed the connection. 
                        {
                            return;
                        }
                    }
                    finally
                    {
                        outputStream.Close();
                    }
                }, new MediaTypeHeaderValue("application/octet-stream"));
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = String.Format("{0}.csv", id)
            };
            return response;
        }

        private string[] GetHeading(DynamicTableEntity dynamicTableEntity)
        {
            return dynamicTableEntity.Properties.Keys.ToArray();
        }

        private async Task StreamContent(byte[] buffer, System.IO.Stream outputStream)
        {
            var pointer = 0;
            while (pointer < buffer.Length)
            {
                var bytesToStream = buffer.Skip(pointer).Take(65536).ToArray();
                await outputStream.WriteAsync(bytesToStream, 0, bytesToStream.Length);
                pointer += bytesToStream.Length;
            }
        }

        private async Task<TableQuerySegment<DynamicTableEntity>> GetData(string id, KeyType keyType, TableContinuationToken token, DateTime? from = null, DateTime? to = null, string filter = "")
        {
            if (to.HasValue && from.HasValue && keyType != KeyType.String)
            {
                return await this.repository.GetTableDataByDate(id, filter, from.Value, to.Value, token, keyType == KeyType.DateTicksLeadingZero);
            }
            else
            {
                return await this.repository.GetTableData(id, filter, token);
            }
        }

        private object GetValue(EntityProperty property)
        {
            switch (property.PropertyType)
            {
                case EdmType.Binary:
                    return property.BinaryValue;
                case EdmType.Boolean:
                    return property.BooleanValue;
                case EdmType.DateTime:
                    return property.DateTimeOffsetValue;
                case EdmType.Double:
                    return property.DoubleValue;
                case EdmType.Guid:
                    return property.GuidValue;
                case EdmType.Int32:
                    return property.Int32Value;
                case EdmType.Int64:
                    return property.Int64Value;
                case EdmType.String:
                    return property.StringValue;
                default:
                    return "";
            }

        }

    }
}