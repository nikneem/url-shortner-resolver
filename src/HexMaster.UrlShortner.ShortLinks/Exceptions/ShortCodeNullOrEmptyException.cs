using HexMaster.UrlShortner.ShortLinks.ErrorCodes;

namespace HexMaster.UrlShortner.ShortLinks.Exceptions;

public class ShortCodeNullOrEmptyException : UrlShortnerShortLinkBaseException
{
    public ShortCodeNullOrEmptyException() : base(UrlShortnerShortLinksErrorCodes.ShortCodeNullOrEmpty)
    {
    }
}