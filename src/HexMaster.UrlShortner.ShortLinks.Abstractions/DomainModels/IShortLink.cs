namespace HexMaster.UrlShortner.ShortLinks.Abstractions.DomainModels;

public interface IShortLink
{
    string ShortCode { get;  }
    string TargetUrl { get;  }
    DateTimeOffset CreatedOn { get; }
    DateTimeOffset? ExpiresOn { get;  }

    void SetShortCode(string value);
    void SetTargetUrl(string value);
    void SetExpiryDate(DateTimeOffset? value);
}
