using HexMaster.UrlShortner.ShortLinks.ErrorCodes;

namespace HexMaster.UrlShortner.ShortLinks.Exceptions;

public class ShortCodeNullOrEmptyException : UrlShortnerShortLinkException
{
    public ShortCodeNullOrEmptyException() : base(UrlShortnerShortLinksErrorCodes.ShortCodeNullOrEmpty)
    {
    }
}