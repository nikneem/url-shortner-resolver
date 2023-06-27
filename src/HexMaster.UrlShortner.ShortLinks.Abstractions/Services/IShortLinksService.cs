using HexMaster.UrlShortner.Core;
using HexMaster.UrlShortner.ShortLinks.Abstractions.DataTransferObjects;

namespace HexMaster.UrlShortner.ShortLinks.Abstractions.Services;

public interface IShortLinksService
{
    Task<ShortLinksListDto> ListAsync(string ownerId, string? query, int pageSize = Constants.DefaultPageSize, int page = 0, CancellationToken cancellationToken = default);
    Task<ShortLinkDetailsDto> GetAsync(string ownerId, Guid id, CancellationToken cancellationToken = default);
    Task<ShortLinkDetailsDto> PostAsync(string ownerId, string targetUrl, CancellationToken cancellationToken = default);
    Task<bool> PutAsync(string ownerId, Guid id, ShortLinkDetailsDto dto, CancellationToken cancellationToken = default);
    Task<ShortLinkDetailsDto> ResolveAsync(string shortCode, CancellationToken cancellationToken = default);
    Task<bool> IsUniqueShortCodeAsync(Guid id, string shortCode, CancellationToken cancellationToken = default);
}
