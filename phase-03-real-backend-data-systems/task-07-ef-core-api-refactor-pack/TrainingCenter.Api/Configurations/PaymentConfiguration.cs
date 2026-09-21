using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Api.Entities;

namespace TrainingCenter.Api.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.PaymentDate)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(p => p.PaymentMethod)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.ReferenceNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.ReferenceNumber)
            .IsUnique();

        builder.Property(p => p.Notes)
            .HasMaxLength(500);

        builder.Property(p => p.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(p => p.CreatedAtUtc)
            .HasDefaultValueSql("GETUTCDATE()");

        // Global Soft-Delete Query Filter
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
