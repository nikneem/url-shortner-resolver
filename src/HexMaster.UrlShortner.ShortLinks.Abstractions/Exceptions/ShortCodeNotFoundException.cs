using HexMaster.UrlShortner.Core.Exceptions;
using HexMaster.UrlShortner.ShortLinks.Abstractions.ErrorCodes;

namespace HexMaster.UrlShortner.ShortLinks.Abstractions.Exceptions;

public class ShortCodeNotFoundException : UrlShortnerBaseException
{
    public ShortCodeNotFoundException(Exception? innerException = null) : base(UrlShortnerShortLinksErrorCodes.ShortLinkNotFound, innerException)
    {
    }
}