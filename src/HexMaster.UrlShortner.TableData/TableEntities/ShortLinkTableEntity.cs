using Azure;
using Azure.Data.Tables;

namespace HexMaster.UrlShortner.TableData.TableEntities;

public class ShortLinkTableEntity : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public string ShortCode { get; set; }
    public Guid OwnerId { get; set; }
    public string EndpointUrl { get; set; }
    public DateTimeOffset CreatedOn{ get; set; }
    public DateTimeOffset? ExpiresOn { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
