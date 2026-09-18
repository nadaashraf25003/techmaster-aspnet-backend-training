using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Api.Entities;

namespace TrainingCenter.Api.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");

        builder.HasKey(s => s.StudentId);
        builder.Property(s => s.StudentId).UseIdentityColumn();

        builder.Property(s => s.FullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(150)
            .IsUnicode(false);

        builder.Property(s => s.PhoneNumber)
            .HasMaxLength(25)
            .IsUnicode(false);

        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasIndex(s => s.Email)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasDatabaseName("UQ_Students_Email");

        builder.HasIndex(s => s.IsActive)
            .HasFilter("[IsDeleted] = 0")
            .HasDatabaseName("IX_Students_IsActive");

        builder.HasQueryFilter(s => !s.IsDeleted);

        builder.HasMany(s => s.Enrollments)
            .WithOne(e => e.Student)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(s => s.Enrollments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
