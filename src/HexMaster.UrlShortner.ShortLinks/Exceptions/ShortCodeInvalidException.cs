using HexMaster.UrlShortner.ShortLinks.ErrorCodes;

namespace HexMaster.UrlShortner.ShortLinks.Exceptions;

public class ShortCodeInvalidException : UrlShortnerShortLinkBaseException
{
    public ShortCodeInvalidException() : base(UrlShortnerShortLinksErrorCodes.ShortLinkInvalid)
    {
    }

}