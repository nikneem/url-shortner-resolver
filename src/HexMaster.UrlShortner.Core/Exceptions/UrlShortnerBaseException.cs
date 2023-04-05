using HexMaster.UrlShortner.Core.ErrorCodes;

namespace HexMaster.UrlShortner.Core.Exceptions;

public class UrlShortnerBaseException : Exception
{
    public UrlShortnerBaseException(UrlShortnerErrorCode errorCode, Exception? innerException = null) : base(errorCode.Code, innerException)
    {

    }
}