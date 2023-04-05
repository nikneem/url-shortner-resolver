using HexMaster.UrlShortner.ShortLinks.ErrorCodes;

namespace HexMaster.UrlShortner.ShortLinks.Exceptions;

public class ShortCodeInvalidException : UrlShortnerShortLinkException
{
    public ShortCodeInvalidException() : base(UrlShortnerShortLinksErrorCodes.ShortLinkInvalid)
    {
    }

}