using HexMaster.UrlShortner.Core.Exceptions;
using HexMaster.UrlShortner.ShortLinks.ErrorCodes;

namespace HexMaster.UrlShortner.ShortLinks.Exceptions;

public class UrlShortnerShortLinkBaseException : UrlShortnerBaseException
{
    public UrlShortnerShortLinkBaseException(UrlShortnerShortLinksErrorCode errorCode, Exception? innerException = null) : base(errorCode, innerException)
    {
    }
}