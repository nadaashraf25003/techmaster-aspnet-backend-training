using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechMaster.Phase03.Task02.Entities;

namespace TechMaster.Phase03.Task02.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments", e =>
        {
            e.HasCheckConstraint("CK_Enrollments_Progress", "[ProgressPercentage] >= 0.00 AND [ProgressPercentage] <= 100.00");
        });

        builder.HasKey(e => e.EnrollmentId);
        builder.Property(e => e.EnrollmentId).UseIdentityColumn();

        builder.Property(e => e.EnrollmentDate)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.Property(e => e.ProgressPercentage)
            .IsRequired()
            .HasPrecision(5, 2)
            .HasDefaultValue(0.00m);

        builder.Property(e => e.FinalResult)
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Unique Composite Constraint to prevent duplicate enrollment
        builder.HasIndex(e => new { e.StudentId, e.TrainingTrackId })
            .IsUnique()
            .HasDatabaseName("UQ_Enrollments_Student_Track");

        builder.HasIndex(e => e.TrainingTrackId)
            .HasDatabaseName("IX_Enrollments_TrackId");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_Enrollments_Status");

        // Relationships
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
