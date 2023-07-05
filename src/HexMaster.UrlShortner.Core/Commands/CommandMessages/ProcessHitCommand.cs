using System.Text.Json;

namespace HexMaster.UrlShortner.Core.Commands.CommandMessages;

public record ProcessHitCommand(
    string ShortCode,
    DateTimeOffset CreatedOn) : IUrlShortnerCommand
{
    public BinaryData ToBinaryData()
    {
        var json = JsonSerializer.Serialize(this);
        return new BinaryData(json);
    }
}
