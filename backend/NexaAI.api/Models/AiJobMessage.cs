namespace NexaAI.Api.Models;

public sealed record AiJobMessage(
    string JobId,
    string JobType,
    string Payload,
    DateTimeOffset CreatedAtUtc
);