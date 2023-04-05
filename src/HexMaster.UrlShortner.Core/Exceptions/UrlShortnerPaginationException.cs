using HexMaster.UrlShortner.Core.ErrorCodes;

namespace HexMaster.UrlShortner.Core.Exceptions;

public class UrlShortnerPaginationException : UrlShortnerBaseException
{
    public UrlShortnerPaginationException(UrlShortnerPaginationErrorCode errorCode) : base(errorCode)
    {
    }
}
