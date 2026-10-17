using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using NexaAI.Api.Models;

namespace NexaAI.Api.Services;

public sealed class SqsMessagePublisher : IAiJobPublisher
{
    private readonly IAmazonSQS _sqs;
    private readonly IConfiguration _configuration;

    public SqsMessagePublisher(
        IAmazonSQS sqs,
        IConfiguration configuration)
    {
        _sqs = sqs;
        _configuration = configuration;
    }

    public async Task PublishAsync(
        AiJobMessage message,
        CancellationToken cancellationToken = default)
    {
        var queueUrl = _configuration["AWS:SqsQueueUrl"];

        if (string.IsNullOrWhiteSpace(queueUrl))
        {
            throw new InvalidOperationException(
                "AWS:SqsQueueUrl is not configured.");
        }

        var request = new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = JsonSerializer.Serialize(message)
        };

        await _sqs.SendMessageAsync(request, cancellationToken);
    }
}