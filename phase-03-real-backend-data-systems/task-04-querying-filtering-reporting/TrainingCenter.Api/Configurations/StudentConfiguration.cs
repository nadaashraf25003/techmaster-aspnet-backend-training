using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Api.Entities;

namespace TrainingCenter.Api.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");

        builder.HasKey(s => s.StudentId);

        builder.Property(s => s.FullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(s => s.Email)
            .IsUnique();

        builder.Property(s => s.PhoneNumber)
            .HasMaxLength(30);

        builder.Property(s => s.Address)
            .HasMaxLength(300);

        builder.Property(s => s.IsActive)
            .HasDefaultValue(true);

        builder.Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Global query filter for soft delete
        builder.HasQueryFilter(s => !s.IsDeleted);

        // Navigation configuration
        builder.HasMany(s => s.Enrollments)
            .WithOne(e => e.Student)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(s => s.Enrollments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
