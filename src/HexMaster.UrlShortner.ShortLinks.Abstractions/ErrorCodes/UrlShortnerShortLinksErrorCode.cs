using HexMaster.UrlShortner.Core.ErrorCodes;

namespace HexMaster.UrlShortner.ShortLinks.Abstractions.ErrorCodes;

public abstract class UrlShortnerShortLinksErrorCode : UrlShortnerErrorCode
{
    public override string ErrorNamespace => $"{base.ErrorNamespace}.ShortLinks";
}