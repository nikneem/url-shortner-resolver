/*
 * This is an Azure Container Apps Job. It is executed using a timer trigger.
 * When executed, this job will accumulate the hits of all short codes in CosmosDb
 * and store them in CosmosDb as a HourlyCumulatedHitTableEntity.
 */
using HexMaster.UrlShortner.TableStorage;

static async Task Main()
{
    Console.WriteLine("Start hourly cumulation of hits in table storage");

    var storageAccountConnectionString = Environment.GetEnvironmentVariable("StorageAccountConnection");
    if (string.IsNullOrWhiteSpace(storageAccountConnectionString))
    {
        Console.WriteLine(
            "No storage account connection string (StorageAccountConnection) found, terminating container");
        return;
    }

    var rawHitsRepository = new ShortLinkHitsRepository(storageAccountConnectionString);
    var hitsReceivedPastHour = await rawHitsRepository.GetAllOfPastHourAsync(CancellationToken.None);

    var hitsPerShortCode = hitsReceivedPastHour.GroupBy(h => h.PartitionKey).Select(g => new
    {
        ShortCode = g.Key,
        Hits = g.Count()
    }).ToList();

    foreach (var hit in hitsPerShortCode)
    {
        Console.WriteLine($"Short code {hit.ShortCode} has {hit.Hits} hits");
    }
}

await Main();