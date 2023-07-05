using HexMaster.UrlShortner.Core.Commands;

namespace HexMaster.UrlShortner.Core.Abstractions;

public interface ICommandsHandler
{
    Task<string> Send(IUrlShortnerCommand command, string queueName, CancellationToken cancellationToken = default);
}