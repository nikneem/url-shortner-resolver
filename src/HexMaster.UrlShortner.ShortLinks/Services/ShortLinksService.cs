using HexMaster.UrlShortner.Core;
using HexMaster.UrlShortner.Core.Helpers;
using HexMaster.UrlShortner.ShortLinks.Abstractions.DataTransferObjects;
using HexMaster.UrlShortner.ShortLinks.Abstractions.Repositories;
using HexMaster.UrlShortner.ShortLinks.Abstractions.Services;
using HexMaster.UrlShortner.ShortLinks.DomainModels;
using HexMaster.UrlShortner.ShortLinks.ErrorCodes;
using HexMaster.UrlShortner.ShortLinks.Exceptions;
using System.Text.RegularExpressions;

namespace HexMaster.UrlShortner.ShortLinks.Services;

public class ShortLinksService : IShortLinksService
{
    private readonly IShortLinksRepository repository;

    public ShortLinksService(IShortLinksRepository repository)
    {
        this.repository = repository;
    }
    public Task<ShortLinksListDto> ListAsync(Guid ownerId, string? query, int pageSize = Constants.DefaultPageSize, int page = 0, CancellationToken cancellationToken = default)
    {
        // Sanitize pagination input and throw exceptions for invalid values
        Sanitize.PaginationInput(page, pageSize);

        if (!string.IsNullOrWhiteSpace(query) && (!Regex.IsMatch(query, Constants.AlphanumericStringRegularExpression)))
        {
            throw new UrlShortnerShortLinkException(UrlShortnerShortLinksErrorCodes.QueryStringInvalid);
        }

        return repository.ListAsync(ownerId, query, pageSize, page, cancellationToken);
    }
    public Task<ShortLinkDetailsDto> GetAsync(Guid ownerId, Guid id, CancellationToken cancellationToken = default)
    {
        return repository.GetAsync(ownerId, id, cancellationToken);
    }
    public async Task<ShortLinkDetailsDto> PostAsync(Guid ownerId, string targetUrl, CancellationToken cancellationToken = default)
    {
        var domainModel = ShortLink.Create(targetUrl);
        if (await repository.UpdateAsync(ownerId, domainModel, cancellationToken))
        {
            return DomainModelToDto(domainModel);
        }
        throw new UrlShortnerShortLinkException(UrlShortnerShortLinksErrorCodes.ShortCodeCreationFailed);
    }
    public async Task<bool> PutAsync(Guid ownerId, Guid id, ShortLinkDetailsDto dto, CancellationToken cancellationToken = default)
    {
        var domainModel = await repository.GetDomainModelAsync(ownerId, id, cancellationToken);
        domainModel.SetShortCode(dto.ShortCode);
        domainModel.SetTargetUrl(dto.TargetUrl);
        domainModel.SetExpiryDate(dto.ExpiresOn);
        return await repository.UpdateAsync(ownerId, domainModel, cancellationToken);
    }

    private static ShortLinkDetailsDto DomainModelToDto(ShortLink domainModel)
    {
        return new ShortLinkDetailsDto
        {
            Id = domainModel.Id,
            ShortCode = domainModel.ShortCode,
            TargetUrl = domainModel.TargetUrl,
            CreatedOn = domainModel.CreatedOn,
            ExpiresOn = domainModel.ExpiresOn
        };
    }

}