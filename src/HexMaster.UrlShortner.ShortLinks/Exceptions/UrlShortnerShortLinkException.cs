using HexMaster.UrlShortner.Core.Exceptions;
using HexMaster.UrlShortner.ShortLinks.ErrorCodes;

namespace HexMaster.UrlShortner.ShortLinks.Exceptions;

public class UrlShortnerShortLinkException : UrlShortnerBaseException
{
    public UrlShortnerShortLinkException(UrlShortnerShortLinksErrorCode errorCode, Exception? innerException = null) : base(errorCode, innerException)
    {
    }
}