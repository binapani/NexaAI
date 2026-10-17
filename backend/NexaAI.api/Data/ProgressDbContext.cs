using Microsoft.EntityFrameworkCore;
using NexaAI.Api.Models.Progress;

namespace NexaAI.Api.Data;

public sealed class ProgressDbContext : DbContext
{
    public ProgressDbContext(DbContextOptions<ProgressDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProgressEntry> ProgressEntries => Set<ProgressEntry>();

    public DbSet<ProgressTask> ProgressTasks => Set<ProgressTask>();

    public DbSet<ProgressPriority> ProgressPriorities => Set<ProgressPriority>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProgressEntry>(entity =>
        {
            entity.HasKey(progress => progress.Id);
            entity.Property(progress => progress.UserId)
                .HasMaxLength(100)
                .IsRequired();
            entity.Property(progress => progress.WorkCompleted)
                .HasMaxLength(5000)
                .IsRequired();
            entity.Property(progress => progress.Blockers)
                .HasMaxLength(5000)
                .IsRequired();
            entity.Property(progress => progress.Notes)
                .HasMaxLength(5000)
                .IsRequired();
            entity.HasIndex(progress => new { progress.UserId, progress.EntryDate })
                .IsUnique();
            entity.HasMany(progress => progress.Tasks)
                .WithOne(task => task.ProgressEntry)
                .HasForeignKey(task => task.ProgressEntryId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(progress => progress.PrioritiesForTomorrow)
                .WithOne(priority => priority.ProgressEntry)
                .HasForeignKey(priority => priority.ProgressEntryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProgressTask>(entity =>
        {
            entity.HasKey(task => task.Id);
            entity.Property(task => task.Title)
                .HasMaxLength(200)
                .IsRequired();
        });

        modelBuilder.Entity<ProgressPriority>(entity =>
        {
            entity.HasKey(priority => priority.Id);
            entity.Property(priority => priority.Description)
                .HasMaxLength(500)
                .IsRequired();
        });
    }
}
