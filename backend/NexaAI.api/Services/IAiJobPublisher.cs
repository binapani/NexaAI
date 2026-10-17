using NexaAI.Api.Models;

namespace NexaAI.Api.Services;

public interface IAiJobPublisher
{
    Task PublishAsync(
        AiJobMessage message,
        CancellationToken cancellationToken = default);
}