namespace HexMaster.UrlShortner.Core.Configuration;

public class AzureCloudConfiguration
{
    public const string SectionName = "Azure";

    public string? StorageAccountName { get; set; }
    public CosmosDbConfiguration? CosmosDb { get; set; }
}