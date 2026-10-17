namespace NexaAI.Api.Services;

public interface IProgressUserContext
{
    string UserId { get; }
}

public sealed class LocalProgressUserContext : IProgressUserContext
{
    public string UserId => "local-user";
}
