using Newtonsoft.Json;

namespace HexMaster.UrlShortner.CosmosDb.TableEntities;

public record HourlyCumulatedHitTableEntity
{
    [JsonProperty("id")] public Guid Id { get; set; }
    [JsonProperty("itemType")] public required string ItemType { get; set; }
    [JsonProperty("shortCode")] public required string ShortCode { get; set; }
    [JsonProperty("hits")] public int Hits { get; set; }
    [JsonProperty("createdOn")] public DateTimeOffset CumulationStartWindow { get; set; }
    [JsonProperty("windowStartOn")] public DateTimeOffset CumulationEndWindow { get; set; }
    [JsonProperty("windowEndOn")] public DateTimeOffset CreatedOn { get; set; }

}