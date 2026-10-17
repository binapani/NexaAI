using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using NexaAI.Api.Models.Progress;
using NexaAI.Api.Services;

namespace NexaAI.Api.Controllers;

[ApiController]
[Route("api/progress")]
public sealed class ProgressController : ControllerBase
{
    private readonly IProgressService _progressService;

    public ProgressController(IProgressService progressService)
    {
        _progressService = progressService;
    }

    [HttpPost]
    [ProducesResponseType<ProgressEntryResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProgressEntryResponse>> Create(
        [FromBody] SaveProgressEntryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var entry = await _progressService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = entry.Id }, entry);
        }
        catch (DailyProgressAlreadyExistsException)
        {
            return Conflict(new ProblemDetails
            {
                Title = "A progress entry already exists for this date.",
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    [HttpGet("today")]
    [ProducesResponseType<ProgressEntryResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProgressEntryResponse>> GetToday(
        CancellationToken cancellationToken)
    {
        var entry = await _progressService.GetTodayAsync(cancellationToken);
        return entry is null
            ? NotFound(new ProblemDetails
            {
                Title = "No progress entry exists for today.",
                Status = StatusCodes.Status404NotFound
            })
            : Ok(entry);
    }

    [HttpGet("history")]
    [ProducesResponseType<IReadOnlyList<ProgressEntryResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<ProgressEntryResponse>>> GetHistory(
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        [FromQuery, Range(1, 100)] int limit = 30,
        [FromQuery, Range(0, int.MaxValue)] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        if (from.HasValue && to.HasValue && from.Value > to.Value)
        {
            ModelState.AddModelError(nameof(from), "'from' must be on or before 'to'.");
            return ValidationProblem(ModelState);
        }

        var entries = await _progressService.GetHistoryAsync(
            from,
            to,
            limit,
            offset,
            cancellationToken);
        return Ok(entries);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ProgressEntryResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProgressEntryResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var entry = await _progressService.GetByIdAsync(id, cancellationToken);
        return entry is null
            ? NotFound(new ProblemDetails
            {
                Title = "Progress entry was not found.",
                Status = StatusCodes.Status404NotFound
            })
            : Ok(entry);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<ProgressEntryResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProgressEntryResponse>> Update(
        Guid id,
        [FromBody] SaveProgressEntryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var entry = await _progressService.UpdateAsync(id, request, cancellationToken);
            return entry is null
                ? NotFound(new ProblemDetails
                {
                    Title = "Progress entry was not found.",
                    Status = StatusCodes.Status404NotFound
                })
                : Ok(entry);
        }
        catch (DailyProgressAlreadyExistsException)
        {
            return Conflict(new ProblemDetails
            {
                Title = "A progress entry already exists for this date.",
                Status = StatusCodes.Status409Conflict
            });
        }
    }
}
