
using Microsoft.AspNetCore.Mvc;
using NexaAI.Api.Models;
using NexaAI.Api.Services;

namespace NexaAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly PythonAiService _pythonAiService;

    public ChatController(PythonAiService pythonAiService)
    {
        _pythonAiService = pythonAiService;
    }

    [HttpPost]
    public async Task<ActionResult<ChatResponse>> SendMessage(
        [FromBody] ChatRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new
            {
                error = "Message cannot be empty."
            });
        }

        try
        {
            var result = await _pythonAiService.SendMessageAsync(
                request.Message,
                request.ConversationId?.ToString(),
                cancellationToken);

            if (!Guid.TryParse(result.ConversationId, out var conversationId))
            {
                return StatusCode(502, new
                {
                    error = "Python AI service returned an invalid conversation ID."
                });
            }

            var response = new ChatResponse
            {
                ConversationId = conversationId,
                Message = request.Message,
                Reply = result.Reply,
                CreatedAtUtc = result.CreatedAtUtc.UtcDateTime
            };

            return Ok(response);
        }
        catch (HttpRequestException)
        {
            return StatusCode(503, new
            {
                error = "The Python AI service is unavailable or returned an error."
            });
        }
        catch (TaskCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            return StatusCode(504, new
            {
                error = "The AI request timed out."
            });
        }
    }
}
