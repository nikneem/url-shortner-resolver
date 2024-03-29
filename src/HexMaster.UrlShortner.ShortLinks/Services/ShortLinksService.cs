﻿using HexMaster.UrlShortner.Core;
using HexMaster.UrlShortner.Core.Helpers;
using HexMaster.UrlShortner.ShortLinks.Abstractions.DataTransferObjects;
using HexMaster.UrlShortner.ShortLinks.Abstractions.Repositories;
using HexMaster.UrlShortner.ShortLinks.Abstractions.Services;
using HexMaster.UrlShortner.ShortLinks.DomainModels;
using System.Text.RegularExpressions;
using HexMaster.UrlShortner.ShortLinks.Abstractions.DomainModels;
using HexMaster.UrlShortner.ShortLinks.Abstractions.ErrorCodes;
using HexMaster.UrlShortner.ShortLinks.Abstractions.Exceptions;
using HexMaster.DomainDrivenDesign;
using HexMaster.UrlShortner.Core.Abstractions;
using HexMaster.UrlShortner.Core.Commands;
using HexMaster.UrlShortner.Core.Commands.CommandMessages;

namespace HexMaster.UrlShortner.ShortLinks.Services;

public class ShortLinksService : IShortLinksService
{
    private readonly IShortLinksRepository _repository;
    private readonly ICommandsHandler _commandsHandler;

    public Task<List<ShortLinksListItemDto>> ListAsync(string ownerId, string? query, CancellationToken cancellationToken = default)
    {
        // Sanitize pagination input and throw exceptions for invalid values
        //Sanitize.PaginationInput(page, pageSize);

        if (!string.IsNullOrWhiteSpace(query) && (!Regex.IsMatch(query, Constants.AlphanumericStringRegularExpression)))
        {
            throw new UrlShortnerShortLinkException(UrlShortnerShortLinksErrorCodes.QueryStringInvalid);
        }

        return _repository.ListAsync(ownerId, query, cancellationToken);
    }
    public Task<ShortLinkDetailsDto> GetAsync(string ownerId, Guid id, CancellationToken cancellationToken = default)
    {
        return _repository.GetAsync(ownerId, id, cancellationToken);
    }
    public async Task<ShortLinkDetailsDto> PostAsync(string ownerId, string targetUrl, CancellationToken cancellationToken = default)
    {
        var uniqueCode = await  GenerateUniqueShortCodeAsync(cancellationToken);
        var domainModel = ShortLink.Create(targetUrl, uniqueCode);
        if (await _repository.UpdateAsync(ownerId, domainModel, cancellationToken))
        {
            return DomainModelToDto(domainModel);
        }
        throw new UrlShortnerShortLinkException(UrlShortnerShortLinksErrorCodes.ShortCodeCreationFailed);
    }
    public async Task<bool> PutAsync(string ownerId, Guid id, ShortLinkDetailsDto dto, CancellationToken cancellationToken = default)
    {
        var domainModel = await _repository.GetDomainModelAsync(ownerId, id, cancellationToken);
        await domainModel.SetShortCode(dto.ShortCode, shortCode => IsUniqueShortCodeAsync(id, shortCode, cancellationToken));
        domainModel.SetTargetUrl(dto.EndpointUrl);
        domainModel.SetExpiryDate(dto.ExpiresOn);
        return await _repository.UpdateAsync(ownerId, domainModel, cancellationToken);
    }

    public async Task<ShortLinkDetailsDto> ResolveAsync(string shortCode, CancellationToken cancellationToken = default)
    {
        var domainModel = await _repository.ResolveAsync(shortCode, cancellationToken);
await             _commandsHandler.Send(new ProcessHitCommand(shortCode, DateTimeOffset.UtcNow), QueueName.HitsQueueName);
        return DomainModelToDto(domainModel);
    }

    public async Task<bool> IsUniqueShortCodeAsync( Guid id, string shortCode, CancellationToken cancellationToken = default)
    {
        return !await _repository.ExistsAsync(id, shortCode, cancellationToken);
    }

    private async Task<string> GenerateUniqueShortCodeAsync(CancellationToken cancellationToken = default)
    {
        var shortCode = Randomizer.GetRandomShortCode();
        var exists = await _repository.ExistsAsync(Guid.Empty, shortCode, cancellationToken);
        if (exists)
        {
            return await GenerateUniqueShortCodeAsync(cancellationToken);
        }

        return shortCode;
    }
    private static ShortLinkDetailsDto DomainModelToDto(IShortLink domainModel)
    {
        if (domainModel is DomainModel<Guid> dm)
        {
            return new ShortLinkDetailsDto
            {
                Id = dm.Id,
                ShortCode = domainModel.ShortCode,
                EndpointUrl = domainModel.TargetUrl,
                CreatedOn = domainModel.CreatedOn,
                ExpiresOn = domainModel.ExpiresOn
            };
        }
        return  null!;
    }

    public ShortLinksService(IShortLinksRepository repository, ICommandsHandler commandsHandler)
    {
        _repository = repository;
        _commandsHandler = commandsHandler;
    }

}