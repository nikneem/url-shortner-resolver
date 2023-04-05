namespace HexMaster.UrlShortner.ShortLinks;

public class Constants
{
    /// <summary>
    /// This regular expression tests for a string that starts with a lowercase
    /// alphabetical character, followed by a alphanumeric string between 1 and 11
    /// characters, making the minimum allows characters 2, and the max allowed 12.
    /// </summary>
    public const string ShortCodeRegularExpression = "^([a-z]{1}[a-z0-9]{1,11})$";
}