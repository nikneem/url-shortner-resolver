using HexMaster.UrlShortner.Core;
using HexMaster.UrlShortner.ShortLinks.Abstractions.DataTransferObjects;

namespace HexMaster.UrlShortner.ShortLinks.Abstractions.Services;

public interface IShortLinksService
{
    Task<ShortLinksListDto> ListAsync(Guid ownerId, string? query, int pageSize = Constants.DefaultPageSize, int page = 0, CancellationToken cancellationToken = default);
    Task<ShortLinkDetailsDto> GetAsync(Guid ownerId, Guid id, CancellationToken cancellationToken = default);
    Task<ShortLinkDetailsDto> PostAsync(Guid ownerId, string targetUrl, CancellationToken cancellationToken = default);
    Task<bool> PutAsync(Guid ownerId, Guid id, ShortLinkDetailsDto dto, CancellationToken cancellationToken = default);
}
