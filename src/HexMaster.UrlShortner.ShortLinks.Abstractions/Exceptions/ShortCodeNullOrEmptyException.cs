using HexMaster.UrlShortner.ShortLinks.Abstractions.ErrorCodes;

namespace HexMaster.UrlShortner.ShortLinks.Abstractions.Exceptions;

public class ShortCodeNullOrEmptyException : UrlShortnerShortLinkException
{
    public ShortCodeNullOrEmptyException() : base(UrlShortnerShortLinksErrorCodes.ShortCodeNullOrEmpty)
    {
    }
}