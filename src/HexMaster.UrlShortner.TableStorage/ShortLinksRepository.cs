using System.Runtime.CompilerServices;
using Azure;
using Azure.Data.Tables;
using HexMaster.DomainDrivenDesign.ChangeTracking;
using HexMaster.DomainDrivenDesign;
using HexMaster.UrlShortner.Core.Configuration;
using HexMaster.UrlShortner.Core.Helpers;
using HexMaster.UrlShortner.ShortLinks.Abstractions.DataTransferObjects;
using HexMaster.UrlShortner.ShortLinks.Abstractions.DomainModels;
using HexMaster.UrlShortner.ShortLinks.Abstractions.Exceptions;
using HexMaster.UrlShortner.ShortLinks.Abstractions.Repositories;
using HexMaster.UrlShortner.ShortLinks.DomainModels;
using HexMaster.UrlShortner.TableStorage.Entities;
using Microsoft.Extensions.Options;

namespace HexMaster.UrlShortner.TableStorage;

public class ShortLinksRepository : IShortLinksRepository
{

    private const string TableName = "shortlinks";
    private readonly TableClient _tableClient;

    public async Task<List<ShortLinksListItemDto>> ListAsync(string ownerId, string? query, CancellationToken cancellationToken) 
    {
        var polls = new List<ShortLinksListItemDto>();
        var pollsQuery = _tableClient.QueryAsync<ShortLinkTableEntity>($"{nameof(ShortLinkTableEntity.PartitionKey)} eq '{ownerId}'");
        await foreach (var queryPage in pollsQuery.AsPages().WithCancellation(cancellationToken))
        {
            polls.AddRange(queryPage.Values.Select(v =>
                new ShortLinksListItemDto
                {
                    Id = Guid.Parse(v.RowKey),
                                       ShortCode = v.ShortCode,
                    EndpointUrl = v.EndpointUrl,
                    ExpiresOn = v.ExpiresOn,
                    CreatedOn = v.Timestamp ?? DateTimeOffset.UtcNow
                }));
        }

        return polls;
    }

    public async Task<ShortLinkDetailsDto> GetAsync(string ownerId, Guid id, CancellationToken cancellationToken)
    {
        var pollsQuery = _tableClient.QueryAsync<ShortLinkTableEntity>($"{nameof(ShortLinkTableEntity.PartitionKey)} eq '{ownerId}' and {nameof(ShortLinkTableEntity.RowKey)} eq '{id}'");
        await foreach (var queryPage in pollsQuery.AsPages().WithCancellation(cancellationToken))
        {
            foreach (var value in queryPage.Values)
            {
                return new ShortLinkDetailsDto
                {
                    Id = Guid.Parse(value.RowKey),
                    ShortCode = value.ShortCode,
                    EndpointUrl = value.EndpointUrl,
                    ExpiresOn = value.ExpiresOn,
                    CreatedOn = value.Timestamp ?? DateTimeOffset.UtcNow
                };
            }
        }

        throw new ShortCodeNotFoundException();
    }

    public async Task<IShortLink> GetDomainModelAsync(string ownerId, Guid id, CancellationToken cancellationToken)
    {
        var pollsQuery = _tableClient.QueryAsync<ShortLinkTableEntity>($"{nameof(ShortLinkTableEntity.PartitionKey)} eq '{ownerId}' and {nameof(ShortLinkTableEntity.RowKey)} eq '{id}'");
        await foreach (var queryPage in pollsQuery.AsPages().WithCancellation(cancellationToken))
        {
            foreach (var value in queryPage.Values)
            {
                return new ShortLink(
                    Guid.Parse(value.RowKey),
                    value.ShortCode,
                    value.EndpointUrl,
                    value.Timestamp ?? DateTimeOffset.UtcNow,
                    value.ExpiresOn
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
                RowKey = dm.Id.ToString(),
                PartitionKey = ownerId,
                EndpointUrl = domainModel.TargetUrl,
                ShortCode = domainModel.ShortCode,
                Timestamp = domainModel.CreatedOn,
                ExpiresOn = domainModel.ExpiresOn
            };

            if (dm.TrackingState == TrackingState.New)
            {
                var response = await _tableClient.AddEntityAsync(entity, cancellationToken);
                return !response.IsError;
            }

            if (dm.TrackingState == TrackingState.Modified)
            {
                var response = await _tableClient.UpdateEntityAsync(entity, ETag.All, TableUpdateMode.Replace, cancellationToken);
                return !response.IsError;
            }
        }

        return false;
    }

    public async Task<bool> ExistsAsync(Guid id, string shortCode, CancellationToken cancellationToken)
    {
        var pollsQuery = _tableClient.QueryAsync<ShortLinkTableEntity>($"{nameof(ShortLinkTableEntity.RowKey)} ne '{id}' and {nameof(ShortLinkTableEntity.ShortCode)} eq '{shortCode}'");
        await foreach (var queryPage in pollsQuery.AsPages().WithCancellation(cancellationToken))
        {
            return queryPage.Values.Any();
        }
        throw new ShortCodeNotFoundException();
    }

    public async Task<IShortLink> ResolveAsync(string shortCode, CancellationToken cancellationToken)
    {
        var pollsQuery = _tableClient.QueryAsync<ShortLinkTableEntity>($"{nameof(ShortLinkTableEntity.ShortCode)} eq '{shortCode}'");
        await foreach (var queryPage in pollsQuery.AsPages().WithCancellation(cancellationToken))
        {
            foreach (var value in queryPage.Values)
            {
                return new ShortLink(
                    Guid.Parse(value.RowKey),
                    value.ShortCode,
                    value.EndpointUrl,
                    value.Timestamp ?? DateTimeOffset.UtcNow,
                    value.ExpiresOn
                );
            }
        }
        throw new ShortCodeNotFoundException();
    }

    public ShortLinksRepository(IOptions<AzureCloudConfiguration> config)
    {
        var identity = CloudIdentity.GetChainedTokenCredential();
        var storageAccountUrl = new Uri($"https://{config.Value.StorageAccountName}.table.core.windows.net");
        _tableClient = new TableClient(storageAccountUrl, TableName, identity);
    }
}