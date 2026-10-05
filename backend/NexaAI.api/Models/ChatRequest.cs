using System.ComponentModel.DataAnnotations;

namespace NexaAI.Api.Models;

public class ChatRequest
{
    [Required]
    [StringLength(4000, MinimumLength = 1)]
    public string Message { get; set; } = string.Empty;

    public Guid? ConversationId { get; set; }
}