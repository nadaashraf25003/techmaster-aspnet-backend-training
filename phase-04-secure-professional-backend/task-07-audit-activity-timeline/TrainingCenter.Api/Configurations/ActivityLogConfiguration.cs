using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Api.Entities;

namespace TrainingCenter.Api.Configurations;

public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.ToTable("ActivityLogs");

        builder.HasKey(a => a.ActivityLogId);

        builder.Property(a => a.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.EntityName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.EntityId)
            .HasMaxLength(100);

        builder.Property(a => a.UserRole)
            .HasMaxLength(50);

        builder.Property(a => a.UserEmail)
            .HasMaxLength(150);

        builder.Property(a => a.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.IpAddress)
            .HasMaxLength(50);

        builder.Property(a => a.CorrelationId)
            .HasMaxLength(100);

        builder.Property(a => a.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Indexes for lightning-fast timeline queries
        builder.HasIndex(a => a.CreatedAt);
        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.EntityName);
        builder.HasIndex(a => a.Action);
    }
}
