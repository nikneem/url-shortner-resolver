using HexMaster.UrlShortner.ShortLinks.Abstractions.ErrorCodes;

namespace HexMaster.UrlShortner.ShortLinks.Abstractions.Exceptions;

public class TargetUrlInvalidException : UrlShortnerShortLinkException
{
    public TargetUrlInvalidException() : base(UrlShortnerShortLinksErrorCodes.TargetUrlInvalid)
    {
    }

}