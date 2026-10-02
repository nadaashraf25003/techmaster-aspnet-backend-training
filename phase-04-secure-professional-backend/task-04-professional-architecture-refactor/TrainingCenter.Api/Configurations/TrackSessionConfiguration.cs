using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Api.Entities;

namespace TrainingCenter.Api.Configurations;

public class TrackSessionConfiguration : IEntityTypeConfiguration<TrackSession>
{
    public void Configure(EntityTypeBuilder<TrackSession> builder)
    {
        builder.ToTable("TrackSessions");

        builder.HasKey(s => s.TrackSessionId);

        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Description)
            .HasMaxLength(1000);

        builder.Property(s => s.MeetingLink)
            .HasMaxLength(500);

        builder.Property(s => s.Notes)
            .HasMaxLength(2000);

        builder.Property(s => s.IsCompleted)
            .HasDefaultValue(false);

        // Relationship with TrainingTrack
        builder.HasOne(s => s.TrainingTrack)
            .WithMany()
            .HasForeignKey(s => s.TrainingTrackId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship with Instructor
        builder.HasOne(s => s.CreatedByInstructor)
            .WithMany()
            .HasForeignKey(s => s.CreatedByInstructorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
