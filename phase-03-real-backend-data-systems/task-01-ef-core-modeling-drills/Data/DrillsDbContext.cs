using EFCoreModelingDrills.Common;
using EFCoreModelingDrills.Drill01_DbContextFirstMigration;
using EFCoreModelingDrills.Drill02_OneToOneStudentProfile;
using EFCoreModelingDrills.Drill03_OneToManyInstructorTracks;
using EFCoreModelingDrills.Drill04_ManyToManyEnrollment;
using EFCoreModelingDrills.Drill05_PaymentSummary;
using EFCoreModelingDrills.Drill06_SeedData;
using Microsoft.EntityFrameworkCore;

namespace EFCoreModelingDrills.Data;

/// <summary>
/// Unified DbContext for EF Core Modeling Drills (Drills 01 to 10).
/// Demonstrates:
/// - DbContext & DbSet mappings (Drill 01)
/// - 1:1 Student <-> StudentProfile (Drill 02)
/// - 1:N Instructor <-> TrainingTrack (Drill 03)
/// - M:N Student + TrainingTrack via Enrollment (Drill 04)
/// - 1:1 Enrollment <-> PaymentSummary with decimal precision (Drill 05)
/// - Deterministic HasData seed configuration (Drill 06)
/// - Soft delete global query filters (Drill 07)
/// - Automated UTC audit tracking interceptor in SaveChangesAsync (Drill 08)
/// </summary>
public class DrillsDbContext : DbContext
{
    public DrillsDbContext(DbContextOptions<DrillsDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<StudentProfile> StudentProfiles => Set<StudentProfile>();
    public DbSet<Instructor> Instructors => Set<Instructor>();
    public DbSet<TrainingTrack> TrainingTracks => Set<TrainingTrack>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<PaymentSummary> PaymentSummaries => Set<PaymentSummary>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ----------------------------------------------------
        // Drill 01 & Drill 07: Student Entity & Soft Delete Filter
        // ----------------------------------------------------
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.FullName).IsRequired().HasMaxLength(150);
            entity.Property(s => s.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(s => s.Email).IsUnique();
            entity.Property(s => s.PhoneNumber).HasMaxLength(30);

            // Drill 07: Global Query Filter for Soft Delete
            entity.HasQueryFilter(s => !s.IsDeleted);
        });

        // ----------------------------------------------------
        // Drill 02: 1:1 Student <-> StudentProfile
        // ----------------------------------------------------
        modelBuilder.Entity<StudentProfile>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.NationalId).IsRequired().HasMaxLength(50);
            entity.Property(p => p.Address).HasMaxLength(250);
            entity.Property(p => p.EmergencyPhone).HasMaxLength(30);

            // 1:1 Relationship Configuration
            entity.HasOne(p => p.Student)
                  .WithOne(s => s.Profile)
                  .HasForeignKey<StudentProfile>(p => p.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(p => p.StudentId).IsUnique();
        });

        // ----------------------------------------------------
        // Drill 03: 1:N Instructor <-> TrainingTrack
        // ----------------------------------------------------
        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.FullName).IsRequired().HasMaxLength(150);
            entity.Property(i => i.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(i => i.Email).IsUnique();
            entity.Property(i => i.Specialization).HasMaxLength(150);

            entity.HasQueryFilter(i => !i.IsDeleted);
        });

        modelBuilder.Entity<TrainingTrack>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Code).IsRequired().HasMaxLength(50);
            entity.HasIndex(t => t.Code).IsUnique();
            entity.Property(t => t.Price).HasPrecision(18, 2);

            // 1:N Relationship Configuration
            entity.HasOne(t => t.Instructor)
                  .WithMany(i => i.Tracks)
                  .HasForeignKey(t => t.InstructorId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(t => !t.IsDeleted);
        });

        // ----------------------------------------------------
        // Drill 04: M:N Enrollment Join Entity Configuration
        // ----------------------------------------------------
        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FinalGrade).HasPrecision(5, 2);

            entity.HasOne(e => e.Student)
                  .WithMany(s => s.Enrollments)
                  .HasForeignKey(e => e.StudentId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.TrainingTrack)
                  .WithMany(t => t.Enrollments)
                  .HasForeignKey(e => e.TrainingTrackId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // ----------------------------------------------------
        // Drill 05: 1:1 Enrollment <-> PaymentSummary
        // ----------------------------------------------------
        modelBuilder.Entity<PaymentSummary>(entity =>
        {
            entity.HasKey(ps => ps.Id);
            entity.Property(ps => ps.TotalRequired).HasPrecision(18, 2);
            entity.Property(ps => ps.TotalPaid).HasPrecision(18, 2);

            entity.HasOne(ps => ps.Enrollment)
                  .WithOne(e => e.PaymentSummary)
                  .HasForeignKey<PaymentSummary>(ps => ps.EnrollmentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(ps => ps.EnrollmentId).IsUnique();
            entity.HasQueryFilter(ps => !ps.IsDeleted);
        });

        // ----------------------------------------------------
        // Drill 06: Apply Seed Data
        // ----------------------------------------------------
        modelBuilder.SeedDrillData();
    }

    // --------------------------------------------------------
    // Drill 08: Automatic UTC Audit Tracking Interceptor
    // --------------------------------------------------------
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseAuditableEntity>();
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.IsDeleted = false;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
    
        return base.SaveChangesAsync(cancellationToken);
    }
}
