using Azure.Data.Tables;
using Azure;

namespace HexMaster.UrlShortner.TableStorage.Entities;

public record ShortLinkHitTableEntity  : ITableEntity
{
    public string RowKey { get; set; }
    public string PartitionKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
