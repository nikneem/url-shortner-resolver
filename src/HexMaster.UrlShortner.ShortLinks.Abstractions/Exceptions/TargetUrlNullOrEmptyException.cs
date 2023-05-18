using HexMaster.UrlShortner.ShortLinks.Abstractions.ErrorCodes;

namespace HexMaster.UrlShortner.ShortLinks.Abstractions.Exceptions;


public class TargetUrlNullOrEmptyException : UrlShortnerShortLinkException
{
    public TargetUrlNullOrEmptyException() : base(UrlShortnerShortLinksErrorCodes.TargetUrlNullOrEmpty)
    {
    }
}
