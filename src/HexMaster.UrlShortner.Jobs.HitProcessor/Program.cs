using System.Text;
using Azure.Storage.Queues;
using HexMaster.UrlShortner.Core.Commands.CommandMessages;
using Newtonsoft.Json;
using HexMaster.UrlShortner.TableStorage;

const string sourceQueueName = "hits";

static async Task Main()
{

    Console.WriteLine("Starting the process job");

    var storageAccountConnectionString = Environment.GetEnvironmentVariable("StorageAccountConnection");
    if (string.IsNullOrWhiteSpace(storageAccountConnectionString))
    {
        Console.WriteLine(
            "No storage account connection string (StorageAccountConnection) found, terminating container");
        return;
    }

    var queueServiceClient = new QueueServiceClient(storageAccountConnectionString);
    var queueClient = queueServiceClient.GetQueueClient(sourceQueueName);

    Console.WriteLine("Receiving message from service bus");
    var receivedMessage = await queueClient.ReceiveMessageAsync();

    if (receivedMessage != null && receivedMessage.HasValue)
    {
        Console.WriteLine("Got a message from the service bus");
        var payloadString = Encoding.UTF8.GetString(receivedMessage.Value.Body);
        var payload = JsonConvert.DeserializeObject<ProcessHitCommand>(payloadString);
        if (payload != null)
        {
            Console.WriteLine("Deserialized to a descent payload");
            var shortLinkHitsRepository = new ShortLinkHitsRepository(storageAccountConnectionString);
            if (await shortLinkHitsRepository.InsertNewHitAsync(
                    payload.ShortCode,
                    payload.CreatedOn,
                    CancellationToken.None))
            {
                Console.WriteLine("Hit stored succesfully, Completing original message in service bus");
                Console.WriteLine("All good, process complete");
            }
        }
        else
        {
            Console.WriteLine("No queue message received, terminating container");
        }
    }
}

await Main();