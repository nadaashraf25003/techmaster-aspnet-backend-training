using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Api.Entities;

namespace TrainingCenter.Api.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Token)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(t => t.Token)
            .IsUnique();

        builder.Property(t => t.ExpiresAtUtc)
            .IsRequired();

        builder.Property(t => t.CreatedAtUtc)
            .IsRequired();

        builder.Property(t => t.CreatedByIp)
            .HasMaxLength(50);

        builder.Property(t => t.RevokedAtUtc);

        builder.Property(t => t.RevokedByIp)
            .HasMaxLength(50);

        builder.Property(t => t.ReplacedByToken)
            .HasMaxLength(200);

        builder.Property(t => t.ReasonRevoked)
            .HasMaxLength(200);
    }
}
