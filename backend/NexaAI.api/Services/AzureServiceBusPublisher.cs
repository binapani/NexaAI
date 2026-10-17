using System.Text.Json;
using Azure.Messaging.ServiceBus;
using NexaAI.Api.Models;

namespace NexaAI.Api.Services;

public sealed class AzureServiceBusPublisher : IAiJobPublisher
{
    private readonly ServiceBusSender _sender;

    public AzureServiceBusPublisher(
        ServiceBusClient client,
        IConfiguration configuration)
    {
        var queueName = configuration["AzureServiceBus:QueueName"];

        if (string.IsNullOrWhiteSpace(queueName))
        {
            throw new InvalidOperationException(
                "AzureServiceBus:QueueName is not configured.");
        }

        _sender = client.CreateSender(queueName);
    }

    public async Task PublishAsync(
        AiJobMessage message,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(message);

        var serviceBusMessage = new ServiceBusMessage(json)
        {
            MessageId = message.JobId,
            ContentType = "application/json",
            Subject = message.JobType
        };

        await _sender.SendMessageAsync(
            serviceBusMessage,
            cancellationToken);
    }
}