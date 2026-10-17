using NexaAI.Api.Models.Progress;

namespace NexaAI.Api.Services;

public interface IProgressService
{
    Task<ProgressEntryResponse> CreateAsync(
        SaveProgressEntryRequest request,
        CancellationToken cancellationToken);

    Task<ProgressEntryResponse?> GetTodayAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<ProgressEntryResponse>> GetHistoryAsync(
        DateOnly? from,
        DateOnly? to,
        int limit,
        int offset,
        CancellationToken cancellationToken);

    Task<ProgressEntryResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<ProgressEntryResponse?> UpdateAsync(
        Guid id,
        SaveProgressEntryRequest request,
        CancellationToken cancellationToken);
}

public sealed class DailyProgressAlreadyExistsException : Exception
{
    public DailyProgressAlreadyExistsException()
        : base("A progress entry already exists for this user and date.")
    {
    }
}
