using Azure.Data.Tables;
using HexMaster.UrlShortner.Core.Configuration;
using HexMaster.UrlShortner.Core.Helpers;
using HexMaster.UrlShortner.TableStorage.Entities;
using Microsoft.Extensions.Options;

namespace HexMaster.UrlShortner.TableStorage;

public class ShortLinkHitsRepository
{

    private const string TableName = "hits";
    private readonly TableClient _tableClient;

    public async Task<bool> InsertNewHitAsync(string shortCode, DateTimeOffset eventDate, CancellationToken token)
    {
        var entity = new ShortLinkHitTableEntity
        {
            RowKey = Guid.NewGuid().ToString(),
            PartitionKey = shortCode,
            Timestamp = eventDate,
        };

        var result = await _tableClient.AddEntityAsync(entity, token);
        return result.Status == 201;
    }

    public async Task<List<ShortLinkHitTableEntity>> GetAllOfPastHourAsync(CancellationToken token)
    {
        var now = DateTimeOffset.UtcNow;
        var endDate = now.AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        var startDate = endDate.AddHours(-1);
        var query = _tableClient.QueryAsync<ShortLinkHitTableEntity>($"Timestamp ge {startDate:o} and Timestamp le {endDate:o}");
        var hits = new List<ShortLinkHitTableEntity>();
        await foreach (var page in query.AsPages().WithCancellation(token))
        {
            hits.AddRange(page.Values);
        }
        return hits;
    }

    public ShortLinkHitsRepository(IOptions<AzureCloudConfiguration> config)
    {
        var identity = CloudIdentity.GetChainedTokenCredential();
        var storageAccountUrl = new Uri($"https://{config.Value.StorageAccountName}.table.core.windows.net");
        _tableClient = new TableClient(storageAccountUrl, TableName, identity);
    }
    public ShortLinkHitsRepository(string storageAccountConnectionString)
    {
        _tableClient = new TableClient(storageAccountConnectionString, TableName);
    }
}