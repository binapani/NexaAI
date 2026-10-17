using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using NexaAI.Api.Models;
using NexaAI.Api.Services;

namespace NexaAI.Api.Controllers;

[ApiController]
[Route("api/ai-jobs")]
public sealed class AiJobsController : ControllerBase
{
   private readonly IAiJobPublisher _publisher;

public AiJobsController(IAiJobPublisher publisher)
    {
        _publisher = publisher;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateJob(
        [FromBody] CreateAiJobRequest request,
        CancellationToken cancellationToken)
    {
        var jobId = Guid.NewGuid().ToString("N");

        var message = new AiJobMessage(
            JobId: jobId,
            JobType: request.JobType,
            Payload: request.Payload,
            CreatedAtUtc: DateTimeOffset.UtcNow
        );

        await _publisher.PublishAsync(message, cancellationToken);

        return Accepted(new
        {
            jobId,
            status = "Queued"
        });
    }
}

public sealed record CreateAiJobRequest(
    [Required, StringLength(100, MinimumLength = 1)]
    string JobType,

    [Required, StringLength(10000, MinimumLength = 1)]
    string Payload
);