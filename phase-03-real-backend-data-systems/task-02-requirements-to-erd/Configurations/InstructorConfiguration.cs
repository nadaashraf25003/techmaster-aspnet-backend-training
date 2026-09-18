using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechMaster.Phase03.Task02.Entities;

namespace TechMaster.Phase03.Task02.Configurations;

public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
{
    public void Configure(EntityTypeBuilder<Instructor> builder)
    {
        builder.ToTable("Instructors");

        builder.HasKey(i => i.InstructorId);
        builder.Property(i => i.InstructorId).UseIdentityColumn();

        builder.Property(i => i.FullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(i => i.Email)
            .IsRequired()
            .HasMaxLength(150)
            .IsUnicode(false);

        builder.Property(i => i.Specialization)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.Bio)
            .HasMaxLength(1000);

        builder.Property(i => i.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(i => i.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(i => i.Email)
            .IsUnique()
            .HasDatabaseName("UQ_Instructors_Email");

        // Relationships
        builder.HasMany(i => i.TrainingTracks)
            .WithOne(t => t.Instructor)
            .HasForeignKey(t => t.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(i => i.TrainingTracks)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
