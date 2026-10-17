using System.Text.Json.Serialization;

namespace NexaAI.Api.Models.Progress;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProgressTaskStatus
{
    Pending,
    InProgress,
    Completed,
    Blocked
}
