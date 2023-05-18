using HexMaster.UrlShortner.ShortLinks.Abstractions.ErrorCodes;

namespace HexMaster.UrlShortner.ShortLinks.Abstractions.Exceptions;

public class ShortCodeInvalidException : UrlShortnerShortLinkException
{
    public ShortCodeInvalidException() : base(UrlShortnerShortLinksErrorCodes.ShortLinkInvalid)
    {
    }

}