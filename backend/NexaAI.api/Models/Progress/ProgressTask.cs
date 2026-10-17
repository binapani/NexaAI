namespace NexaAI.Api.Models.Progress;

public sealed class ProgressTask
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ProgressEntryId { get; set; }

    public ProgressEntry ProgressEntry { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    public ProgressTaskStatus Status { get; set; }

    public int SortOrder { get; set; }
}
