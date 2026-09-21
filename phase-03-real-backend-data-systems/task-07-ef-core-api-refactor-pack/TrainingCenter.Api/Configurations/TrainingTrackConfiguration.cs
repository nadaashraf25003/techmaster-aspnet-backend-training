using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Api.Entities;

namespace TrainingCenter.Api.Configurations;

public class TrainingTrackConfiguration : IEntityTypeConfiguration<TrainingTrack>
{
    public void Configure(EntityTypeBuilder<TrainingTrack> builder)
    {
        builder.ToTable("TrainingTracks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(t => t.Code)
            .IsUnique();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.Price)
            .HasColumnType("decimal(18,2)");

        builder.Property(t => t.Capacity)
            .IsRequired();

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(t => t.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(t => t.CreatedAtUtc)
            .HasDefaultValueSql("GETUTCDATE()");

        // Global Soft-Delete Query Filter
        builder.HasQueryFilter(t => !t.IsDeleted);

        builder.HasMany(t => t.Enrollments)
            .WithOne(e => e.TrainingTrack)
            .HasForeignKey(e => e.TrainingTrackId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
