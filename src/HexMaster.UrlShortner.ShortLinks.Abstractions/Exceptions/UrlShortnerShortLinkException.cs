using HexMaster.UrlShortner.Core.Exceptions;
using HexMaster.UrlShortner.ShortLinks.Abstractions.ErrorCodes;

namespace HexMaster.UrlShortner.ShortLinks.Abstractions.Exceptions;

public class UrlShortnerShortLinkException : UrlShortnerBaseException
{
    public UrlShortnerShortLinkException(UrlShortnerShortLinksErrorCode errorCode, Exception? innerException = null) : base(errorCode, innerException)
    {
    }
}