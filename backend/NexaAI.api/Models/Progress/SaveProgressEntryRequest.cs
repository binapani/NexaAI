using System.ComponentModel.DataAnnotations;

namespace NexaAI.Api.Models.Progress;

public sealed class SaveProgressEntryRequest : IValidatableObject
{
    [Required]
    public DateOnly EntryDate { get; init; }

    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public List<ProgressTaskRequest> Tasks { get; init; } = [];

    [Required(AllowEmptyStrings = true)]
    [StringLength(5000)]
    public string WorkCompleted { get; init; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [StringLength(5000)]
    public string Blockers { get; init; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public List<ProgressPriorityRequest> PrioritiesForTomorrow { get; init; } = [];

    [Required(AllowEmptyStrings = true)]
    [StringLength(5000)]
    public string Notes { get; init; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EntryDate == default)
        {
            yield return new ValidationResult(
                "EntryDate must be a valid date.",
                [nameof(EntryDate)]);
        }
    }
}

public sealed class ProgressTaskRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; init; } = string.Empty;

    [EnumDataType(typeof(ProgressTaskStatus))]
    public ProgressTaskStatus Status { get; init; }
}

public sealed class ProgressPriorityRequest
{
    [Required]
    [StringLength(500, MinimumLength = 1)]
    public string Description { get; init; } = string.Empty;
}
