using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Api.Entities;

namespace TrainingCenter.Api.Configurations;

public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
{
    public void Configure(EntityTypeBuilder<Instructor> builder)
    {
        builder.ToTable("Instructors");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.FullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(i => i.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(i => i.Email)
            .IsUnique();

        builder.Property(i => i.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(i => i.Specialization)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.HourlyRate)
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.IsActive)
            .HasDefaultValue(true);

        builder.Property(i => i.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(i => i.CreatedAtUtc)
            .HasDefaultValueSql("GETUTCDATE()");

        // Global Soft-Delete Query Filter
        builder.HasQueryFilter(i => !i.IsDeleted);

        builder.HasMany(i => i.TrainingTracks)
            .WithOne(t => t.Instructor)
            .HasForeignKey(t => t.InstructorId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
