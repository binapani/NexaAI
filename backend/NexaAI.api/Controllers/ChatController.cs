
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using Polly.Timeout;
using NexaAI.Api.Models;
using NexaAI.Api.Services;

namespace NexaAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly PythonAiService _pythonAiService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(
        PythonAiService pythonAiService,
        ILogger<ChatController> logger)
    {
        _pythonAiService = pythonAiService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ChatResponse>> SendMessage(
        [FromBody] ChatRequest request,
        CancellationToken cancellationToken)
    {
        // 1. Validate input before calling the AI service.
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new
            {
                error = "Message cannot be empty."
            });
        }

        try
        {
            // 2. Forward the cancellation token to PythonAiService.
            // This allows cancellation to propagate through the HTTP call.
            var result = await _pythonAiService.SendMessageAsync(
                request.Message,
                request.ConversationId?.ToString(),
                cancellationToken);

            // 3. Validate the conversation ID returned by Python.
            if (!Guid.TryParse(
                result.ConversationId,
                out var conversationId))
            {
                _logger.LogWarning(
                    "Python AI service returned an invalid conversation ID.");

                return StatusCode(502, new
                {
                    error = "Python AI service returned an invalid conversation ID."
                });
            }

            // 4. Preserve the existing response contract for Angular.
            var response = new ChatResponse
            {
                ConversationId = conversationId,
                Message = request.Message,
                Reply = result.Reply,
                CreatedAtUtc = result.CreatedAtUtc.UtcDateTime
            };

            return Ok(response);
        }
        // 5. The circuit breaker is open.
        // Fail fast instead of exposing internal exception details.
        catch (BrokenCircuitException ex)
        {
            _logger.LogWarning(
                ex,
                "Chat request rejected because the Python AI circuit is open.");

            return StatusCode(503, new
            {
                error = "The AI service is temporarily unavailable. Please try again shortly."
            });
        }
        // 6. The resilience pipeline rejected the request because
        // its timeout strategy expired.
        catch (TimeoutRejectedException ex)
        {
            _logger.LogWarning(
                ex,
                "Chat request exceeded the configured AI timeout.");

            return StatusCode(504, new
            {
                error = "The AI request timed out. Please try again."
            });
        }
        // 7. The Python service could not be reached, or an HTTP
        // error was raised by the downstream request.
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(
                ex,
                "Unable to complete the request to the Python AI service.");

            return StatusCode(503, new
            {
                error = "The AI service is unavailable. Please try again shortly."
            });
        }
        // 8. A timeout or cancellation occurred.
        // Only return a gateway-timeout response when the request
        // itself was not cancelled by the client.
        catch (OperationCanceledException ex)
            when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(
                ex,
                "Chat request was cancelled because the downstream operation timed out.");

            return StatusCode(504, new
            {
                error = "The AI request timed out. Please try again."
            });
        }
        // 9. Unexpected failures should be logged on the server,
        // while the client receives a generic response.
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while processing a chat request.");

            return StatusCode(500, new
            {
                error = "An unexpected error occurred while processing your request."
            });
        }
    }
}
