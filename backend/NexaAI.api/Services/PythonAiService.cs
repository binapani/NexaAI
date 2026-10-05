using System.Net.Http.Json;

namespace NexaAI.Api.Services;

public sealed class PythonAiService
{
    private readonly HttpClient _httpClient;

    public PythonAiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

     public async Task<PythonChatResponse> SendMessageAsync(
        string message,
        string? conversationId,
        CancellationToken cancellationToken = default)
    {
        var request = new PythonChatRequest(message, conversationId);

        using var response = await _httpClient.PostAsJsonAsync(
            "chat",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<PythonChatResponse>(
                cancellationToken: cancellationToken);

        return result ?? throw new InvalidOperationException(
            "Python AI service returned an empty response.");
    }
}

public sealed record PythonChatRequest(
    string Message,
    string? ConversationId);

public sealed record PythonChatResponse(
    string ConversationId,
    string Reply,
    DateTimeOffset CreatedAtUtc);