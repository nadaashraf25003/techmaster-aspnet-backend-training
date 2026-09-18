using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Api.Entities;

namespace TrainingCenter.Api.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.PaymentId);

        // Rule P1 & P2: decimal(18,2) precision
        builder.Property(p => p.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.PaymentMethod)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(p => p.PaymentStatus)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(p => p.ReferenceNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(p => p.ReferenceNumber)
            .IsUnique();

        builder.Property(p => p.PaymentDate)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(p => p.Notes)
            .HasMaxLength(500);

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Global query filter for soft delete
        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.HasIndex(p => p.PaymentDate);
        builder.HasIndex(p => p.PaymentStatus);

        builder.HasOne(p => p.Enrollment)
            .WithMany(e => e.Payments)
            .HasForeignKey(p => p.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
