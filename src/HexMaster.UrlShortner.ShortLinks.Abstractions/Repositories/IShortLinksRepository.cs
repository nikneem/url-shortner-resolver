using HexMaster.UrlShortner.ShortLinks.Abstractions.DataTransferObjects;
using HexMaster.UrlShortner.ShortLinks.Abstractions.DomainModels;

namespace HexMaster.UrlShortner.ShortLinks.Abstractions.Repositories;

public interface IShortLinksRepository
{
    Task<ShortLinksListDto> ListAsync(Guid ownerId, string? query, int pageSize, int page, CancellationToken cancellationToken);
    Task<ShortLinkDetailsDto> GetAsync(Guid ownerId, Guid id, CancellationToken cancellationToken);
    Task<IShortLink> GetDomainModelAsync(Guid ownerId, Guid id, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Guid ownerId, IShortLink domainModel, CancellationToken cancellationToken);
}
