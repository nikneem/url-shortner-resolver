namespace HexMaster.UrlShortner.Messaging.Commands;

public record ProcessHitCommand(
    string ShortCode, 
    DateTimeOffset CreatedOn);
