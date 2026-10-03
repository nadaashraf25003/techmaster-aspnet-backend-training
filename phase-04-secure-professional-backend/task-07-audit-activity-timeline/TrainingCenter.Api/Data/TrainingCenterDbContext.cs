using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common;
using TrainingCenter.Api.Entities;

namespace TrainingCenter.Api.Data;

public class TrainingCenterDbContext : DbContext
{
    public TrainingCenterDbContext(DbContextOptions<TrainingCenterDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Instructor> Instructors => Set<Instructor>();
    public DbSet<TrainingTrack> TrainingTracks => Set<TrainingTrack>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<TrackSession> TrackSessions => Set<TrackSession>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<TrackSession>(entity =>
        {
            entity.HasOne(s => s.TrainingTrack)
                .WithMany(t => t.TrackSessions)
                .HasForeignKey(s => s.TrainingTrackId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(s => s.CreatedByInstructor)
                .WithMany()
                .HasForeignKey(s => s.CreatedByInstructorId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseAuditableEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastModifiedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
