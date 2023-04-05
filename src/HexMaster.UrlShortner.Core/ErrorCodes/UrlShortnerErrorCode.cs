namespace HexMaster.UrlShortner.Core.ErrorCodes;

public abstract class UrlShortnerErrorCode
{
    public abstract string Code { get; }
    public virtual string TranslationKey => $"{ErrorNamespace}.{Code}";
    public virtual string ErrorNamespace => "ErrorCodes";
}

