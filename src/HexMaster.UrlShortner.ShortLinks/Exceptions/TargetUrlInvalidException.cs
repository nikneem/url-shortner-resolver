using HexMaster.UrlShortner.ShortLinks.ErrorCodes;

namespace HexMaster.UrlShortner.ShortLinks.Exceptions;

public class TargetUrlInvalidException : UrlShortnerShortLinkException
{
    public TargetUrlInvalidException() : base(UrlShortnerShortLinksErrorCodes.TargetUrlInvalid)
    {
    }

}