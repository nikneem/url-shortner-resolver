namespace HexMaster.UrlShortner.Core.ErrorCodes;

public abstract class UrlShortnerPaginationErrorCode: UrlShortnerErrorCode
{
    public override string ErrorNamespace => $"{base.ErrorNamespace}.Pagination";
}
