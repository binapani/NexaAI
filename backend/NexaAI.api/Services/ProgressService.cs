using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NexaAI.Api.Data;
using NexaAI.Api.Models.Progress;

namespace NexaAI.Api.Services;

public sealed class ProgressService : IProgressService
{
    private readonly ProgressDbContext _dbContext;
    private readonly IProgressUserContext _userContext;
    private readonly TimeProvider _timeProvider;

    public ProgressService(
        ProgressDbContext dbContext,
        IProgressUserContext userContext,
        TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _timeProvider = timeProvider;
    }

    public async Task<ProgressEntryResponse> CreateAsync(
        SaveProgressEntryRequest request,
        CancellationToken cancellationToken)
    {
        var alreadyExists = await _dbContext.ProgressEntries.AnyAsync(
            entry => entry.UserId == _userContext.UserId
                && entry.EntryDate == request.EntryDate,
            cancellationToken);

        if (alreadyExists)
        {
            throw new DailyProgressAlreadyExistsException();
        }

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var entry = new ProgressEntry
        {
            UserId = _userContext.UserId,
            EntryDate = request.EntryDate,
            WorkCompleted = request.WorkCompleted,
            Blockers = request.Blockers,
            Notes = request.Notes,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };
        ReplaceChildren(entry, request);

        _dbContext.ProgressEntries.Add(entry);
        await SaveEntryAsync(cancellationToken);
        return ToResponse(entry);
    }

    public async Task<ProgressEntryResponse?> GetTodayAsync(
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(_timeProvider.GetLocalNow().DateTime);
        var entry = await GetDetailedEntries()
            .SingleOrDefaultAsync(
                item => item.UserId == _userContext.UserId && item.EntryDate == today,
                cancellationToken);

        return entry is null ? null : ToResponse(entry);
    }

    public async Task<IReadOnlyList<ProgressEntryResponse>> GetHistoryAsync(
        DateOnly? from,
        DateOnly? to,
        int limit,
        int offset,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(_timeProvider.GetLocalNow().DateTime);
        var query = GetDetailedEntries()
            .Where(entry => entry.UserId == _userContext.UserId
                && entry.EntryDate < today);

        if (from.HasValue)
        {
            query = query.Where(entry => entry.EntryDate >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(entry => entry.EntryDate <= to.Value);
        }

        var entries = await query
            .OrderByDescending(entry => entry.EntryDate)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return entries.Select(ToResponse).ToArray();
    }

    public async Task<ProgressEntryResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var entry = await GetDetailedEntries()
            .SingleOrDefaultAsync(
                item => item.Id == id && item.UserId == _userContext.UserId,
                cancellationToken);

        return entry is null ? null : ToResponse(entry);
    }

    public async Task<ProgressEntryResponse?> UpdateAsync(
        Guid id,
        SaveProgressEntryRequest request,
        CancellationToken cancellationToken)
    {
        var entry = await _dbContext.ProgressEntries
            .Include(item => item.Tasks)
            .Include(item => item.PrioritiesForTomorrow)
            .SingleOrDefaultAsync(
                item => item.Id == id && item.UserId == _userContext.UserId,
                cancellationToken);

        if (entry is null)
        {
            return null;
        }

        var dateAlreadyExists = await _dbContext.ProgressEntries.AnyAsync(
            item => item.UserId == _userContext.UserId
                && item.EntryDate == request.EntryDate
                && item.Id != id,
            cancellationToken);

        if (dateAlreadyExists)
        {
            throw new DailyProgressAlreadyExistsException();
        }

        entry.EntryDate = request.EntryDate;
        entry.WorkCompleted = request.WorkCompleted;
        entry.Blockers = request.Blockers;
        entry.Notes = request.Notes;
        entry.UpdatedAtUtc = _timeProvider.GetUtcNow().UtcDateTime;
        ReplaceChildren(entry, request);

        await SaveEntryAsync(cancellationToken);
        return ToResponse(entry);
    }

    private IQueryable<ProgressEntry> GetDetailedEntries()
    {
        return _dbContext.ProgressEntries
            .AsNoTracking()
            .Include(entry => entry.Tasks)
            .Include(entry => entry.PrioritiesForTomorrow);
    }

    private void ReplaceChildren(ProgressEntry entry, SaveProgressEntryRequest request)
    {
        entry.Tasks.Clear();
        entry.PrioritiesForTomorrow.Clear();
        _dbContext.ChangeTracker.DetectChanges();

        for (var index = 0; index < request.Tasks.Count; index++)
        {
            var task = request.Tasks[index];
            entry.Tasks.Add(new ProgressTask
            {
                Title = task.Title.Trim(),
                Status = task.Status,
                SortOrder = index
            });
        }

        for (var index = 0; index < request.PrioritiesForTomorrow.Count; index++)
        {
            var priority = request.PrioritiesForTomorrow[index];
            entry.PrioritiesForTomorrow.Add(new ProgressPriority
            {
                Description = priority.Description.Trim(),
                SortOrder = index
            });
        }
    }

    private async Task SaveEntryAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
        {
            throw new DailyProgressAlreadyExistsException();
        }
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        return exception.InnerException is SqliteException
        {
            SqliteExtendedErrorCode: 2067
        };
    }

    private static ProgressEntryResponse ToResponse(ProgressEntry entry)
    {
        return new ProgressEntryResponse(
            entry.Id,
            entry.EntryDate,
            entry.Tasks
                .OrderBy(task => task.SortOrder)
                .Select(task => new ProgressTaskResponse(task.Title, task.Status))
                .ToArray(),
            entry.WorkCompleted,
            entry.Blockers,
            entry.PrioritiesForTomorrow
                .OrderBy(priority => priority.SortOrder)
                .Select(priority => priority.Description)
                .ToArray(),
            entry.Notes,
            entry.CreatedAtUtc,
            entry.UpdatedAtUtc);
    }
}
