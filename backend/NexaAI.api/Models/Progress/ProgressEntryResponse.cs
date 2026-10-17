namespace NexaAI.Api.Models.Progress;

public sealed record ProgressEntryResponse(
    Guid Id,
    DateOnly EntryDate,
    IReadOnlyList<ProgressTaskResponse> Tasks,
    string WorkCompleted,
    string Blockers,
    IReadOnlyList<string> PrioritiesForTomorrow,
    string Notes,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);

public sealed record ProgressTaskResponse(
    string Title,
    ProgressTaskStatus Status);
