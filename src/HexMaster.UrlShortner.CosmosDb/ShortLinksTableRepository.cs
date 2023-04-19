using HexMaster.UrlShortner.Core.Configuration;
using HexMaster.UrlShortner.Core.Helpers;
using HexMaster.UrlShortner.CosmosDb.TableEntities;
using HexMaster.UrlShortner.ShortLinks.Abstractions.DataTransferObjects;
using HexMaster.UrlShortner.ShortLinks.Abstractions.DomainModels;
using HexMaster.UrlShortner.ShortLinks.Abstractions.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using Microsoft.Azure.Cosmos.Linq;

namespace HexMaster.UrlShortner.CosmosDb;

public class ShortLinksTableRepository : IShortLinksRepository
{
    private const string DatabaseName = "shortlinks`";
    private const string ContainerName = "items";
    private readonly CosmosClient _cosmosClient;

    public async Task<ShortLinksListDto> ListAsync(Guid ownerId, string? query, int pageSize, int page,
        CancellationToken cancellationToken)
    {
        var responseObject = new ShortLinksListDto
        {
            Page = page,
            PageSize = pageSize,
            TotalPages = 0,
            TotalRecords = 0,
            ShortLinks = new List<ShortLinksListItemDto>()
        };

        var container = _cosmosClient.GetContainer(DatabaseName, ContainerName);

        int skip = pageSize * page;
        var linkQueryDefinition = container.GetItemLinqQueryable<ShortLinkTableEntity>()
            .Where(x => x.OwnerId == ownerId)
            .OrderByDescending(x => x.CreatedOn)
            .Skip(skip)
            .Take(pageSize);

        if (!string.IsNullOrWhiteSpace(query))
        {
            linkQueryDefinition =
                linkQueryDefinition.Where(x => x.ShortCode.Contains(query) || x.EndpointUrl.Contains(query));
        }

        using FeedIterator<ShortLinkTableEntity> linqFeed = linkQueryDefinition.ToFeedIterator();
        try
        {
            while (linqFeed.HasMoreResults)
            {
                FeedResponse<ShortLinkTableEntity> response = await linqFeed.ReadNextAsync(cancellationToken);

                responseObject.ShortLinks.AddRange(response.ToList().Select(item => new ShortLinksListItemDto
                {
                    Id = Guid.Parse(item.RowKey),
                    ShortCode = item.ShortCode,
                    TargetUrl = item.EndpointUrl,
                    CreatedOn = item.CreatedOn,
                    ExpiresOn = item.ExpiresOn
                }));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return responseObject;
    }


    public async Task<ShortLinkDetailsDto> GetAsync(Guid ownerId, Guid id, CancellationToken cancellationToken)
    {
        //var entity = await _tableClient.GetEntityAsync<ShortLinkTableEntity>(PartitionKey, id.ToString(),  cancellationToken: cancellationToken);
        //if (entity != null && entity.Value.OwnerId == ownerId)
        //{
        //    return new ShortLinkDetailsDto()
        //    {
        //        Id = Guid.Parse(entity.Value.RowKey),
        //        ShortCode = entity.Value.ShortCode,
        //        TargetUrl = entity.Value.EndpointUrl,
        //        CreatedOn = entity.Value.CreatedOn,
        //        ExpiresOn = entity.Value.ExpiresOn
        //    };
        //}

        //return default;
        throw new NotImplementedException();
    }

    public async Task<IShortLink> GetDomainModelAsync(Guid ownerId, Guid id, CancellationToken cancellationToken)
    {
        //var entity = await _tableClient.GetEntityAsync<ShortLinkTableEntity>(PartitionKey, id.ToString(), cancellationToken: cancellationToken);
        //if (entity != null && entity.Value.OwnerId == ownerId)
        //{
        //    return new ShortLink(
        //        Guid.Parse(entity.Value.RowKey),
        //        entity.Value.ShortCode,
        //        entity.Value.EndpointUrl,
        //        entity.Value.CreatedOn,
        //        entity.Value.ExpiresOn
        //    );
        //}

        //return default;

        throw new NotImplementedException();

    }

    public async Task<bool> UpdateAsync(Guid ownerId, IShortLink domainModel, CancellationToken cancellationToken)
    {
        //var entity = await _tableClient.GetEntityAsync<ShortLinkTableEntity>(PartitionKey, id.ToString(), cancellationToken: cancellationToken);
        //if (entity != null && entity.Value.OwnerId == ownerId)
        //{
        //    return new ShortLink(
        //        Guid.Parse(entity.Value.RowKey),
        //        entity.Value.ShortCode,
        //        entity.Value.EndpointUrl,
        //        entity.Value.CreatedOn,
        //        entity.Value.ExpiresOn
        //    );
        //}

        //return default;

        throw new NotImplementedException();

    }

    public ShortLinksTableRepository(IOptions<AzureCloudConfiguration> cloudConfiguration)
    {
        // Here, I want to create a CloudTableClient instance using the StorageAccountName from the cloudConfiguration
        var identity = CloudIdentity.GetChainedTokenCredential();
        _cosmosClient= new CosmosClient("https://shortlink-api-we-cdb.documents.azure.com:443/", "lMH7DWdyUadgU8DlvqdDi2vckmV1NB5RA2nqVkBfRWde6kXZ4GVM7t8KQUZdk6KWwnhXSZzTLwlyACDbVrHwyQ=="); // identity
    }
}