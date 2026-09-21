using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Api.Entities;

namespace TrainingCenter.Api.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.EnrollmentDate)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(e => e.CreatedAtUtc)
            .HasDefaultValueSql("GETUTCDATE()");

        // Unique index preventing duplicate active enrollments per student per track
        builder.HasIndex(e => new { e.StudentId, e.TrainingTrackId })
            .IsUnique();

        // Global Soft-Delete Query Filter
        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.HasMany(e => e.Payments)
            .WithOne(p => p.Enrollment)
            .HasForeignKey(p => p.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
