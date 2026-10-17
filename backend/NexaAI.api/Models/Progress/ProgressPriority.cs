namespace NexaAI.Api.Models.Progress;

public sealed class ProgressPriority
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ProgressEntryId { get; set; }

    public ProgressEntry ProgressEntry { get; set; } = null!;

    public string Description { get; set; } = string.Empty;

    public int SortOrder { get; set; }
}
