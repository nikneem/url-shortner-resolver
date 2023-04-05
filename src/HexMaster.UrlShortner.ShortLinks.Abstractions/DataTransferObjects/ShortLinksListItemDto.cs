﻿namespace HexMaster.UrlShortner.ShortLinks.Abstractions.DataTransferObjects;

public record ShortLinksListItemDto
{

    public required Guid Id { get; init; }
    public required string ShortCode { get; init; }
    public required string TargetUrl { get; init; }
    public required DateTimeOffset CreatedOn { get; init; }
    public DateTimeOffset? ExpiresOn { get; init; }

}
