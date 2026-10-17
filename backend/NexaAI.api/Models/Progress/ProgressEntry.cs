namespace NexaAI.Api.Models.Progress;

public sealed class ProgressEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string UserId { get; set; } = string.Empty;

    public DateOnly EntryDate { get; set; }

    public ICollection<ProgressTask> Tasks { get; set; } = new List<ProgressTask>();

    public string WorkCompleted { get; set; } = string.Empty;

    public string Blockers { get; set; } = string.Empty;

    public ICollection<ProgressPriority> PrioritiesForTomorrow { get; set; }
        = new List<ProgressPriority>();

    public string Notes { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}
