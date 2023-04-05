using System.Text.RegularExpressions;
using HexMaster.DomainDrivenDesign;
using HexMaster.DomainDrivenDesign.ChangeTracking;
using HexMaster.UrlShortner.ShortLinks.Exceptions;

namespace HexMaster.UrlShortner.ShortLinks.DomainModels;

public class ShortLink : DomainModel<Guid>
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

    public ShortLink(Guid id, string shortCode, string targetUrl, DateTimeOffset createdOn, DateTimeOffset? expiresOn) : base(id)
    {
        ShortCode = shortCode;
        TargetUrl = targetUrl;
        CreatedOn = createdOn;
        ExpiresOn = expiresOn;
    }

    private ShortLink(string targetUrl) : base(Guid.NewGuid(), TrackingState.New)
    {
        ShortCode = "";// Generate Random
        TargetUrl = targetUrl;
        CreatedOn = DateTimeOffset.UtcNow;
    }

    public static ShortLink Create(string targetUrl)
    {
        return new ShortLink(targetUrl);
    }
}
