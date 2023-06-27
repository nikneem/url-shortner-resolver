namespace HexMaster.UrlShortner.ShortLinks.Abstractions.DataTransferObjects;

public class ShortLinkDetailsDto
{
    public required Guid Id { get; init; }
    public required string ShortCode { get; init; }
    public required string TargetUrl { get; init; }
    public DateTimeOffset? CreatedOn { get; init; }
    public DateTimeOffset? ExpiresOn { get; init; }
}
