namespace HexMaster.UrlShortner.ShortLinks.ErrorCodes;

public abstract class UrlShortnerShortLinksErrorCodes : UrlShortnerShortLinksErrorCode
{
    public static readonly UrlShortnerShortLinksErrorCode ShortCodeNullOrEmpty = new ShortCodeNullOrEmptyErrorCode();
    public static readonly UrlShortnerShortLinksErrorCode ShortLinkInvalid = new ShortCodeInvalidErrorCode();
}