using Azure.Storage.Queues;
using HexMaster.UrlShortner.Core.Abstractions;
using HexMaster.UrlShortner.Core.Configuration;
using HexMaster.UrlShortner.Core.Helpers;
using Microsoft.Extensions.Options;

namespace HexMaster.UrlShortner.Core.Commands;

public class UrlShortnerCommands : ICommandsHandler
{
    private readonly QueueServiceClient _queueServiceClient;

    public async Task<string> Send(IUrlShortnerCommand command, string queueName, CancellationToken cancellationToken = default)
    {
        var queueClient = _queueServiceClient.GetQueueClient(queueName);
        var binaryMessage = command.ToBinaryData();
        var response = await queueClient.SendMessageAsync(binaryMessage, cancellationToken: cancellationToken);
        return response.Value.MessageId;
    }

    public UrlShortnerCommands(IOptions<AzureCloudConfiguration> configuration)
    {
        var identity = CloudIdentity.GetChainedTokenCredential();
        var storageAccountUrl = new Uri($"https://{configuration.Value.StorageAccountName}.queue.core.windows.net");
        _queueServiceClient = new QueueServiceClient(storageAccountUrl, identity);
    }
}