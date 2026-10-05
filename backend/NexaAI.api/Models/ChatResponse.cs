namespace NexaAI.Api.Models;

public class ChatResponse
{
    public Guid ConversationId { get; set; }

    public string Message { get; set; } = string.Empty;

    public string Reply { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }
}