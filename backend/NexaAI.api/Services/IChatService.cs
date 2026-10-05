using NexaAI.Api.Models;

namespace NexaAI.Api.Services;

public interface IChatService
{
    Task<ChatResponse> SendAsync(
        ChatRequest request,
        CancellationToken cancellationToken = default);
}