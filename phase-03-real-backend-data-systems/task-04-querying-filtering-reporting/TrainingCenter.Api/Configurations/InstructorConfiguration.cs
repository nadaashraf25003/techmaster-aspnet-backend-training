using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Api.Entities;

namespace TrainingCenter.Api.Configurations;

public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
{
    public void Configure(EntityTypeBuilder<Instructor> builder)
    {
        builder.ToTable("Instructors");

        builder.HasKey(i => i.InstructorId);

        builder.Property(i => i.FullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(i => i.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(i => i.Email)
            .IsUnique();

        builder.Property(i => i.PhoneNumber)
            .HasMaxLength(30);

        builder.Property(i => i.Specialization)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.HourlyRate)
            .HasPrecision(18, 2);

        builder.Property(i => i.IsActive)
            .HasDefaultValue(true);

        builder.Property(i => i.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Global query filter for soft delete
        builder.HasQueryFilter(i => !i.IsDeleted);

        // Navigation configuration
        builder.HasMany(i => i.TrainingTracks)
            .WithOne(t => t.PrimaryInstructor)
            .HasForeignKey(t => t.PrimaryInstructorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Navigation(i => i.TrainingTracks)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
