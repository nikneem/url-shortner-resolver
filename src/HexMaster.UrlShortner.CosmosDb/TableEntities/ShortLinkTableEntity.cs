using Newtonsoft.Json;

namespace HexMaster.UrlShortner.CosmosDb.TableEntities;

public record ShortLinkTableEntity
{
    [JsonProperty("id")] public Guid Id { get; set; }
    [JsonProperty("itemType")] public required string ItemType { get; set; }
    [JsonProperty("ownerId")] public string OwnerId { get; set; }
    [JsonProperty("shortCode")] public required string ShortCode { get; set; }
    [JsonProperty("endpointUrl")] public required string EndpointUrl { get; set; }
    [JsonProperty("createdOn")] public DateTimeOffset CreatedOn { get; set; }
    [JsonProperty("expiresOn")] public DateTimeOffset? ExpiresOn { get; set; }
}