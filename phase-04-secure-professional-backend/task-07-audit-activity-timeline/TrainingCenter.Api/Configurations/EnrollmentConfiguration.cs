using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Api.Entities;

namespace TrainingCenter.Api.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");

        builder.HasKey(e => e.EnrollmentId);

        // Rule E1: Unique index on StudentId + TrainingTrackId
        builder.HasIndex(e => new { e.StudentId, e.TrainingTrackId })
            .IsUnique();

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.ProgressPercentage)
            .HasPrecision(5, 2)
            .HasDefaultValue(0.00m);

        builder.Property(e => e.FinalResult)
            .HasMaxLength(50);

        builder.Property(e => e.Notes)
            .HasMaxLength(500);

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Global query filter for soft delete
        builder.HasQueryFilter(e => !e.IsDeleted);

        // Navigations
        builder.HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TrainingTrack)
            .WithMany(t => t.Enrollments)
            .HasForeignKey(e => e.TrainingTrackId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Payments)
            .WithOne(p => p.Enrollment)
            .HasForeignKey(p => p.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(e => e.Payments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
