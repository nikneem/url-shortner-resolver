using HexMaster.UrlShortner.Core.Exceptions;
using HexMaster.UrlShortner.ShortLinks.Abstractions.ErrorCodes;

namespace HexMaster.UrlShortner.ShortLinks.Abstractions.Exceptions;

public class ShortCodeNotUniqueException : UrlShortnerBaseException
{
    public ShortCodeNotUniqueException(Exception? innerException = null) : base(UrlShortnerShortLinksErrorCodes.ShortCodeNotUnique, innerException)
    {
    }

}