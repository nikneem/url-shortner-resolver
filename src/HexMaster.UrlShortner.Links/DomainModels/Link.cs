using HexMaster.DomainDrivenDesign;
using HexMaster.DomainDrivenDesign.ChangeTracking;

namespace HexMaster.UrlShortner.Links.DomainModels;

public class Link : DomainModel<Guid>
{

    public required string ShortCode { get; private set; }
    public required string TargetUrl { get; private set; }
    public required DateTimeOffset CreatedOn { get; }
    public DateTimeOffset? ExpiresOn { get; private set; }

    public Link(Guid id, string shortCode, string targetUrl, DateTimeOffset createdOn, DateTimeOffset? expiresOn) : base(id)
    {
    }
}
