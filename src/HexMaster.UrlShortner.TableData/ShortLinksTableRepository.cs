using System.Linq.Expressions;
using Azure.Data.Tables;
using HexMaster.UrlShortner.Core.Configuration;
using HexMaster.UrlShortner.Core.Helpers;
using HexMaster.UrlShortner.ShortLinks.Abstractions.DataTransferObjects;
using HexMaster.UrlShortner.ShortLinks.Abstractions.DomainModels;
using HexMaster.UrlShortner.ShortLinks.Abstractions.Repositories;
using HexMaster.UrlShortner.ShortLinks.DomainModels;
using HexMaster.UrlShortner.TableData.TableEntities;
using Microsoft.Extensions.Options;

namespace HexMaster.UrlShortner.TableData;

public class ShortLinksTableRepository : IShortLinksRepository
{
    private const string TableName = "shortlinks";
    private const string PartitionKey = "shortlink";
    private readonly TableClient _tableClient;

    public async Task<ShortLinksListDto> ListAsync(Guid ownerId, string? query, int pageSize, int page,
        CancellationToken cancellationToken)
    {
        var baseFilter = TableClient.CreateQueryFilter<ShortLinkTableEntity>(ent => ent.OwnerId == ownerId);
        if (!string.IsNullOrWhiteSpace(query))
        {
            var queryFilter = TableClient.CreateQueryFilter<ShortLinkTableEntity>(ent => ent.ShortCode == query || ent.EndpointUrl == query);
            baseFilter = $"({baseFilter}) and ({queryFilter})";
        }

        var shortLinkEntitiesQuery = _tableClient.QueryAsync<ShortLinkTableEntity>(baseFilter,
            maxPerPage: pageSize,
            cancellationToken: cancellationToken);

        var shortLinksList = new ShortLinksListDto
        {
            Page = page,
            PageSize = pageSize,
            TotalPages = 0,
            TotalRecords = 0,
            ShortLinks = new List<ShortLinksListItemDto>()
        };

        await foreach (var shortLinksPage in shortLinkEntitiesQuery.AsPages())
        {
            shortLinksList.ShortLinks.AddRange(shortLinksPage.Values.Select(x => new ShortLinksListItemDto
            {
                Id = Guid.Parse(x.RowKey),
                ShortCode = x.ShortCode,
                TargetUrl = x.EndpointUrl,
                CreatedOn = x.CreatedOn,
                ExpiresOn = x.ExpiresOn
            }));
        }

        return shortLinksList;
    }


    public async Task<ShortLinkDetailsDto> GetAsync(Guid ownerId, Guid id, CancellationToken cancellationToken)
    {
        var entity = await _tableClient.GetEntityAsync<ShortLinkTableEntity>(PartitionKey, id.ToString(),  cancellationToken: cancellationToken);
        if (entity != null && entity.Value.OwnerId == ownerId)
        {
            return new ShortLinkDetailsDto()
            {
                Id = Guid.Parse(entity.Value.RowKey),
                ShortCode = entity.Value.ShortCode,
                TargetUrl = entity.Value.EndpointUrl,
                CreatedOn = entity.Value.CreatedOn,
                ExpiresOn = entity.Value.ExpiresOn
            };
        }

        return default;
    }

    public async Task<IShortLink> GetDomainModelAsync(Guid ownerId, Guid id, CancellationToken cancellationToken)
    {
        var entity = await _tableClient.GetEntityAsync<ShortLinkTableEntity>(PartitionKey, id.ToString(), cancellationToken: cancellationToken);
        if (entity != null && entity.Value.OwnerId == ownerId)
        {
            return new ShortLink(
                Guid.Parse(entity.Value.RowKey),
                entity.Value.ShortCode,
                entity.Value.EndpointUrl,
                entity.Value.CreatedOn,
                entity.Value.ExpiresOn
            );
        }

        return default;
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
        var tableServiceUrl = new Uri($"https://{cloudConfiguration.Value.StorageAccountName}.table.core.windows.net");
        _tableClient = new TableClient(tableServiceUrl, TableName, identity);
    }
}