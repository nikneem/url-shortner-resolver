using HexMaster.UrlShortner.ShortLinks.ErrorCodes;

namespace HexMaster.UrlShortner.ShortLinks.Exceptions;


public class TargetUrlNullOrEmptyException : UrlShortnerShortLinkException
{
    public TargetUrlNullOrEmptyException() : base(UrlShortnerShortLinksErrorCodes.TargetUrlNullOrEmpty)
    {
    }
}
