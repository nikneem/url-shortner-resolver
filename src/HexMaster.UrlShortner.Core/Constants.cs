namespace HexMaster.UrlShortner.Core;

public class Constants
{

    public const int DefaultPageSize = 25;

    /// <summary>
    /// This regular expression tests for a string that starts with a lowercase
    /// alphabetical character, followed by a alphanumeric string between 1 and 11
    /// characters, making the minimum allows characters 2, and the max allowed 12.
    /// </summary>
    public const string ShortCodeRegularExpression = "^([a-z]{1}[a-z0-9]{1,11})$";
    public const string UrlRegularExpression = "^(?<Protocol>(http|https)+):\\/\\/(?<Domain>[\\w@][\\w.:@]+)\\/?[\\w\\.?=%&=\\-@/$,]*";
    public const string AlphanumericStringRegularExpression = "^\\w+$";
}