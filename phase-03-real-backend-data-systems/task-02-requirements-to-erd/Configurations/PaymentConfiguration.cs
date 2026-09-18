using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechMaster.Phase03.Task02.Entities;

namespace TechMaster.Phase03.Task02.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments", p =>
        {
            p.HasCheckConstraint("CK_Payments_Amount", "[Amount] > 0.00");
        });

        builder.HasKey(p => p.PaymentId);
        builder.Property(p => p.PaymentId).UseIdentityColumn();

        builder.Property(p => p.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.PaymentMethod)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsUnicode(false);

        builder.Property(p => p.PaymentDate)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.Property(p => p.PaymentStatus)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.Property(p => p.ReferenceNumber)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(p => p.Notes)
            .HasMaxLength(500);

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(p => p.ReferenceNumber)
            .IsUnique()
            .HasDatabaseName("UQ_Payments_ReferenceNumber");

        builder.HasIndex(p => p.EnrollmentId)
            .HasDatabaseName("IX_Payments_EnrollmentId");

        builder.HasIndex(p => p.PaymentStatus)
            .HasDatabaseName("IX_Payments_PaymentStatus");

        // Relationship
        builder.HasOne(p => p.Enrollment)
            .WithMany(e => e.Payments)
            .HasForeignKey(p => p.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
