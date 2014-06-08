using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Web.Storage.Contracts;
using CsvHelper;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.Web.Storage
{
    public class StorageCSVProcesser : IStorageCSVProcesser
    {
        public async Task<string> GetCSV(IList<DynamicTableEntity> results, string[] headings, bool addHeaderRow)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                {
                    using (var streamReader = new StreamReader(memoryStream))
                    {
                        using (var writer = new CsvWriter(streamWriter))
                        {
                            for (int i = 0; i < results.Count; i++)
                            {
                                var item = results[i];
                                if (i == 0)
                                {
                                    if (addHeaderRow)
                                    {
                                        writer.WriteField("PartitionKey");
                                        writer.WriteField("RowKey");
                                        writer.WriteField("TimeStamp");
                                        foreach (var key in headings)
                                        {
                                            writer.WriteField(key);
                                        }
                                        writer.NextRecord();
                                    }
                                }

                                writer.WriteField(item.PartitionKey);
                                writer.WriteField(item.RowKey);
                                writer.WriteField(item.Timestamp);

                                foreach (var key in headings)
                                {
                                    var prop = item.Properties.ContainsKey(key)
                                                   ? GetValue(item.Properties[key]).ToString()
                                                   : string.Empty;
                                    writer.WriteField(prop);
                                }

                                writer.NextRecord();
                            }

                            streamWriter.Flush();
                            memoryStream.Position = 0;

                            return await streamReader.ReadToEndAsync();
                        }
                    }
                }
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
