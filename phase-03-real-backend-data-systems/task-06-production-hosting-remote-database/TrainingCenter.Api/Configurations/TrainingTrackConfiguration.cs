using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Api.Entities;

namespace TrainingCenter.Api.Configurations;

public class TrainingTrackConfiguration : IEntityTypeConfiguration<TrainingTrack>
{
    public void Configure(EntityTypeBuilder<TrainingTrack> builder)
    {
        builder.ToTable("TrainingTracks");

        builder.HasKey(t => t.TrainingTrackId);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Code)
            .IsRequired()
            .HasMaxLength(50);

        // Rule T1: Unique Track Code Index
        builder.HasIndex(t => t.Code)
            .IsUnique();

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.Price)
            .HasPrecision(18, 2);

        builder.Property(t => t.Capacity)
            .HasDefaultValue(30);

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(t => t.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Global query filter for soft delete
        builder.HasQueryFilter(t => !t.IsDeleted);

        // Foreign keys & navigations
        builder.HasOne(t => t.PrimaryInstructor)
            .WithMany(i => i.TrainingTracks)
            .HasForeignKey(t => t.PrimaryInstructorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(t => t.Enrollments)
            .WithOne(e => e.TrainingTrack)
            .HasForeignKey(e => e.TrainingTrackId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(t => t.Enrollments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
