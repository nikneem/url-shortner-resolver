using HexMaster.UrlShortner.Core.ErrorCodes;
using HexMaster.UrlShortner.Core.Exceptions;

namespace HexMaster.UrlShortner.Core.Helpers;

public static class Sanitize
{
    public static void PaginationInput(int page, int pageSize)
    {
        if (page < 0)
        {
            throw new UrlShortnerPaginationException(new UrlShortnerPaginationInvalidPageNumberErrorCode());
        }
        if (pageSize < 10 || pageSize > 100)
        {
            throw new UrlShortnerPaginationException(new UrlShortnerPaginationInvalidPageSizeErrorCode());
        }
    }
}
