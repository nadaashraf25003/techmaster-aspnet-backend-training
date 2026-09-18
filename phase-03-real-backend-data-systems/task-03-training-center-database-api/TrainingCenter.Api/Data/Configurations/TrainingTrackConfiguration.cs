using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Api.Entities;

namespace TrainingCenter.Api.Data.Configurations;

public class TrainingTrackConfiguration : IEntityTypeConfiguration<TrainingTrack>
{
    public void Configure(EntityTypeBuilder<TrainingTrack> builder)
    {
        builder.ToTable("TrainingTracks", t =>
        {
            t.HasCheckConstraint("CK_TrainingTracks_Capacity", "[Capacity] > 0");
            t.HasCheckConstraint("CK_TrainingTracks_Price", "[Price] >= 0.00");
            t.HasCheckConstraint("CK_TrainingTracks_Dates", "[EndDate] >= [StartDate]");
        });

        builder.HasKey(t => t.TrainingTrackId);
        builder.Property(t => t.TrainingTrackId).UseIdentityColumn();

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Code)
            .IsRequired()
            .HasMaxLength(30)
            .IsUnicode(false);

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.Level)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.Property(t => t.Price)
            .IsRequired()
            .HasPrecision(18, 2)
            .HasDefaultValue(0.00m);

        builder.Property(t => t.Capacity)
            .IsRequired();

        builder.Property(t => t.StartDate)
            .IsRequired();

        builder.Property(t => t.EndDate)
            .IsRequired();

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.Property(t => t.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasIndex(t => t.Code)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasDatabaseName("UQ_TrainingTracks_Code");

        builder.HasIndex(t => t.InstructorId)
            .HasDatabaseName("IX_TrainingTracks_InstructorId");

        builder.HasIndex(t => t.Status)
            .HasFilter("[IsDeleted] = 0")
            .HasDatabaseName("IX_TrainingTracks_Status");

        builder.HasQueryFilter(t => !t.IsDeleted);

        builder.HasOne(t => t.Instructor)
            .WithMany(i => i.TrainingTracks)
            .HasForeignKey(t => t.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Enrollments)
            .WithOne(e => e.TrainingTrack)
            .HasForeignKey(e => e.TrainingTrackId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(t => t.Enrollments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
