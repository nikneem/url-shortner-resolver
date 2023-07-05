using Newtonsoft.Json;

namespace HexMaster.UrlShortner.CosmosDb.TableEntities;

public record HitTableEntity
{
    [JsonProperty("id")] public Guid Id { get; set; }
    [JsonProperty("itemType")] public required string ItemType { get; set; }
    [JsonProperty("shortCode")] public required string ShortCode { get; set; }
    [JsonProperty("createdOn")] public DateTimeOffset CreatedOn { get; set; }
}