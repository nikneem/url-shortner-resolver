namespace HexMaster.UrlShortner.Core.Helpers;

public static class Randomizer
{

    public static string GetRandomShortCode()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }


}