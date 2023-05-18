﻿using System.Text.RegularExpressions;
using HexMaster.DomainDrivenDesign;
using HexMaster.DomainDrivenDesign.ChangeTracking;
using HexMaster.UrlShortner.Core;
using HexMaster.UrlShortner.ShortLinks.Abstractions.DomainModels;
using HexMaster.UrlShortner.ShortLinks.Abstractions.Exceptions;

namespace HexMaster.UrlShortner.ShortLinks.DomainModels;

public class ShortLink : DomainModel<Guid>, IShortLink
{

    public string ShortCode { get; private set; }
    public string TargetUrl { get; private set; }
    public DateTimeOffset CreatedOn { get; }
    public DateTimeOffset? ExpiresOn { get; private set; }

    public void SetShortCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ShortCodeNullOrEmptyException();
        }

        if (!Regex.IsMatch(value, Constants.ShortCodeRegularExpression, RegexOptions.Compiled | RegexOptions.IgnoreCase))
        {
            throw new ShortCodeInvalidException();
        }

        if (!Equals(ShortCode, value))
        {
            ShortCode = value.ToLowerInvariant();
            SetState(TrackingState.Modified);
        }
    }
    public void SetTargetUrl(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new TargetUrlNullOrEmptyException();
        }

        if (!Regex.IsMatch(value, Constants.UrlRegularExpression, RegexOptions.Compiled | RegexOptions.IgnoreCase))
        {
            throw new TargetUrlInvalidException();
        }

        if (!Equals(TargetUrl, value))
        {
            TargetUrl = value.ToLowerInvariant();
            SetState(TrackingState.Modified);
        }
    }
    public void SetExpiryDate(DateTimeOffset? value)
    {
        if (!Equals(ExpiresOn, value))
        {
            ExpiresOn = value;
            SetState(TrackingState.Modified);
        }
    }

    public ShortLink(Guid id, string shortCode, string targetUrl, DateTimeOffset createdOn, DateTimeOffset? expiresOn) : base(id)
    {
        ShortCode = shortCode;
        TargetUrl = targetUrl;
        CreatedOn = createdOn;
        ExpiresOn = expiresOn;
    }

    private ShortLink(string targetUrl, string shortCode) : base(Guid.NewGuid(), TrackingState.New)
    {
        ShortCode = shortCode;
        TargetUrl = targetUrl;
        CreatedOn = DateTimeOffset.UtcNow;
    }

    public static ShortLink Create(string targetUrl, string shortCode)
    {
        return new ShortLink(targetUrl, shortCode);
    }
}
