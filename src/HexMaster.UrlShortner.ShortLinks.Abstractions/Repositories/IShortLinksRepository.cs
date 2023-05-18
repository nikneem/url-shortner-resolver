using HexMaster.UrlShortner.ShortLinks.Abstractions.DataTransferObjects;
using HexMaster.UrlShortner.ShortLinks.Abstractions.DomainModels;

namespace HexMaster.UrlShortner.ShortLinks.Abstractions.Repositories;

public interface IShortLinksRepository
{
    Task<ShortLinksListDto> ListAsync(string ownerId, string? query, int pageSize, int page, CancellationToken cancellationToken);
    Task<ShortLinkDetailsDto> GetAsync(string ownerId, Guid id, CancellationToken cancellationToken);
    Task<IShortLink> GetDomainModelAsync(string ownerId, Guid id, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(string ownerId, IShortLink domainModel, CancellationToken cancellationToken);
    Task<bool> ExistsAsync( string shortCode, CancellationToken cancellationToken);
    Task<IShortLink> ResolveAsync(string shortCode, CancellationToken cancellationToken);
}
