using HexMaster.DomainDrivenDesign;
using HexMaster.DomainDrivenDesign.ChangeTracking;
using HexMaster.UrlShortner.Core.Configuration;
using HexMaster.UrlShortner.Core.ExtensionMethods;
using HexMaster.UrlShortner.Core.Helpers;
using HexMaster.UrlShortner.CosmosDb.TableEntities;
using HexMaster.UrlShortner.ShortLinks.Abstractions.DataTransferObjects;
using HexMaster.UrlShortner.ShortLinks.Abstractions.DomainModels;
using HexMaster.UrlShortner.ShortLinks.Abstractions.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Microsoft.Azure.Cosmos.Linq;
using HexMaster.UrlShortner.ShortLinks.Abstractions.Exceptions;
using HexMaster.UrlShortner.ShortLinks.DomainModels;

namespace HexMaster.UrlShortner.CosmosDb;

public class ShortLinksTableRepository : IShortLinksRepository
{
    private const string DatabaseName = "shortlinks";
    private const string ContainerName = "operation";
    private const string ItemType = "shortlink";
    private readonly CosmosClient _cosmosClient;

    public async Task<ShortLinksListDto> ListAsync(
        string ownerId, 
        string? query, 
        int pageSize, 
        int page,
        CancellationToken cancellationToken)
    {
        var container = _cosmosClient.GetContainer(DatabaseName, ContainerName);

        int skip = pageSize * page;
        var linkQueryDefinition = container.GetItemLinqQueryable<ShortLinkTableEntity>()
            .Where(x => x.OwnerId == ownerId && x.ItemType == ItemType);

        if (!string.IsNullOrWhiteSpace(query))
        {
            linkQueryDefinition =
                linkQueryDefinition.Where(x => x.ShortCode.Contains(query) || x.EndpointUrl.Contains(query));
        }


        var totalEntities = await linkQueryDefinition.CountAsync(cancellationToken: cancellationToken);
        linkQueryDefinition = linkQueryDefinition.OrderByDescending(ent=> ent.CreatedOn).Skip(skip).Take(pageSize);

        var responseObject = new ShortLinksListDto
        {
            Page = page,
            PageSize = pageSize,
            TotalPages = totalEntities.Resource.CalculatePages(pageSize),
            TotalRecords = totalEntities.Resource,
            ShortLinks = new List<ShortLinksListItemDto>()
        };



        using FeedIterator<ShortLinkTableEntity> linqFeed = linkQueryDefinition.ToFeedIterator();
        try
        {
            while (linqFeed.HasMoreResults)
            {
                FeedResponse<ShortLinkTableEntity> response = await linqFeed.ReadNextAsync(cancellationToken);

                responseObject.ShortLinks.AddRange(response.ToList().Select(item => new ShortLinksListItemDto
                {
                    Id = item.Id,
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


    public async Task<ShortLinkDetailsDto> GetAsync(string ownerId, Guid id, CancellationToken cancellationToken)
    {
        var container = _cosmosClient.GetContainer(DatabaseName, ContainerName);

        var queryString = $"SELECT * FROM c WHERE c.{nameof(ShortLinkTableEntity.ItemType).LowerCaseFirstCharecter()} = @partitionKey AND c.{nameof(ShortLinkTableEntity.Id).LowerCaseFirstCharecter()} = @id AND c.{nameof(ShortLinkTableEntity.OwnerId).LowerCaseFirstCharecter()} = @ownerId";
        QueryDefinition query =
            new QueryDefinition(queryString)
                .WithParameter("@partitionKey", ItemType)
                .WithParameter("@id", id)
                .WithParameter("@ownerId", ownerId);
        var queryResultSetIterator = container.GetItemQueryIterator<ShortLinkTableEntity>(query);

        while (queryResultSetIterator.HasMoreResults)
        {
            var currentResultSet = await queryResultSetIterator.ReadNextAsync(cancellationToken);
            Console.WriteLine($"Found {currentResultSet.Count} results in the result set");
            foreach (var entity in currentResultSet)
            {
                return new ShortLinkDetailsDto
                {
                    Id = entity.Id,
                    ShortCode = entity.ShortCode,
                    TargetUrl = entity.EndpointUrl,
                    CreatedOn = entity.CreatedOn,
                    ExpiresOn = entity.ExpiresOn
                };
            }
        }

        throw new ShortCodeNotFoundException();
    }

    public async Task<IShortLink> GetDomainModelAsync(string ownerId, Guid id, CancellationToken cancellationToken)
    {
        var container = _cosmosClient.GetContainer(DatabaseName, ContainerName);

        var queryString = $"SELECT * FROM c WHERE c.{nameof(ShortLinkTableEntity.ItemType).LowerCaseFirstCharecter()} = @partitionKey AND c.{nameof(ShortLinkTableEntity.Id).LowerCaseFirstCharecter()} = @id AND c.{nameof(ShortLinkTableEntity.OwnerId).LowerCaseFirstCharecter()} = @ownerId";
        QueryDefinition query =
            new QueryDefinition(queryString)
                .WithParameter("@partitionKey", ItemType)
                .WithParameter("@id", id)
                .WithParameter("@ownerId", ownerId);
        var queryResultSetIterator = container.GetItemQueryIterator<ShortLinkTableEntity>(query);

        while (queryResultSetIterator.HasMoreResults)
        {
            var currentResultSet = await queryResultSetIterator.ReadNextAsync(cancellationToken);
            foreach (var entity in currentResultSet)
            {
                return new ShortLink(
                    entity.Id,
                    entity.ShortCode,
                    entity.EndpointUrl,
                    entity.CreatedOn,
                    entity.ExpiresOn
                );
            }
        }

        throw new ShortCodeNotFoundException();
    }

    public async Task<bool> UpdateAsync(string ownerId, IShortLink domainModel, CancellationToken cancellationToken)
    {
        if (domainModel is DomainModel<Guid> dm)
        {
            var entity = new ShortLinkTableEntity
            {
                Id = dm.Id,
                ItemType = ItemType,
                OwnerId = ownerId,
                EndpointUrl = domainModel.TargetUrl,
                ShortCode = domainModel.ShortCode,
                CreatedOn = domainModel.CreatedOn,
                ExpiresOn = domainModel.ExpiresOn
            };

            var container = _cosmosClient.GetContainer(DatabaseName, ContainerName);

            if (dm.TrackingState == TrackingState.New)
            {
                var response = await container.CreateItemAsync(entity, cancellationToken: cancellationToken);
                return response.StatusCode == System.Net.HttpStatusCode.Created;
            }

            if (dm.TrackingState == TrackingState.Modified)
            {
                var response = await container.ReplaceItemAsync(entity, entity.Id.ToString(), cancellationToken: cancellationToken);
                return response.StatusCode == System.Net.HttpStatusCode.OK;
            }

        }

        //var entity = await _tableClient.GetEntityAsync<ShortLinkTableEntity>(ItemType, id.ToString(), cancellationToken: cancellationToken);
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

    public async Task<bool> ExistsAsync(string shortCode, CancellationToken cancellationToken)
    {
        var container = _cosmosClient.GetContainer(DatabaseName, ContainerName);

        var linkQueryDefinition = container.GetItemLinqQueryable<ShortLinkTableEntity>()
            .Where(x => x.ItemType == ItemType && x.ShortCode == shortCode);

        var totalEntities = await linkQueryDefinition.CountAsync(cancellationToken: cancellationToken);
        return totalEntities.Resource > 0;
    }

    public async Task<IShortLink> ResolveAsync(string shortCode, CancellationToken cancellationToken)
    {
        var container = _cosmosClient.GetContainer(DatabaseName, ContainerName);
        var queryable = container.GetItemLinqQueryable<ShortLinkTableEntity>();

        var matches = queryable
            .Where(ent => ent.ItemType == ItemType)
            .Where(ent => ent.ShortCode == shortCode)
            .Where(ent => ent.ExpiresOn == null || ent.ExpiresOn > DateTime.UtcNow);

        var linqFeed = matches.ToFeedIterator();
        if (linqFeed.HasMoreResults)
        {
            var responseFeed = await linqFeed.ReadNextAsync(cancellationToken);
            var entity = responseFeed.FirstOrDefault();
            if (entity != null)
            {
                return new ShortLink(
                    entity.Id,
                    entity.ShortCode,
                    entity.EndpointUrl,
                    entity.CreatedOn,
                    entity.ExpiresOn
                );
            }
        }

        throw new ShortCodeNotFoundException();

    }

    public ShortLinksTableRepository(IOptions<AzureCloudConfiguration> cloudConfiguration)
    {
        var cosmosDbConfiguration = cloudConfiguration.Value.CosmosDb;
        var cosmosDbEndpoint = cosmosDbConfiguration?.Endpoint;
        var cosmosDbKey = cosmosDbConfiguration?.Key;
        if (string.IsNullOrWhiteSpace(cosmosDbEndpoint))
        {
            throw new ArgumentNullException(nameof(cosmosDbEndpoint));
        }

        if (string.IsNullOrWhiteSpace(cosmosDbKey))
        {
            var identity = CloudIdentity.GetChainedTokenCredential();
            _cosmosClient = new CosmosClient(cosmosDbEndpoint, identity);
        }
        else
        {
            _cosmosClient = new CosmosClient(cosmosDbEndpoint, cosmosDbKey);
        }
    }
}