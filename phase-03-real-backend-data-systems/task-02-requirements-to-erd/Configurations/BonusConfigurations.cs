using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechMaster.Phase03.Task02.Entities.Bonus;

namespace TechMaster.Phase03.Task02.Configurations;

public class TrackSessionConfiguration : IEntityTypeConfiguration<TrackSession>
{
    public void Configure(EntityTypeBuilder<TrackSession> builder)
    {
        builder.ToTable("TrackSessions");

        builder.HasKey(s => s.SessionId);
        builder.Property(s => s.SessionId).UseIdentityColumn();

        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(s => s.RoomOrLink)
            .HasMaxLength(255);

        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasIndex(s => new { s.TrainingTrackId, s.SessionDate })
            .HasDatabaseName("IX_TrackSessions_TrackDate");

        builder.HasOne(s => s.TrainingTrack)
            .WithMany(t => t.Sessions)
            .HasForeignKey(s => s.TrainingTrackId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(s => s.Attendances)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.ToTable("Attendances");

        builder.HasKey(a => a.AttendanceId);
        builder.Property(a => a.AttendanceId).UseIdentityColumn();

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.Property(a => a.Remarks)
            .HasMaxLength(255);

        builder.Property(a => a.MarkedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.Property(a => a.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasIndex(a => new { a.TrackSessionId, a.StudentId })
            .IsUnique()
            .HasDatabaseName("UQ_Attendances_Session_Student");

        builder.HasIndex(a => a.StudentId)
            .HasDatabaseName("IX_Attendances_StudentId");

        builder.HasOne(a => a.TrackSession)
            .WithMany(s => s.Attendances)
            .HasForeignKey(a => a.TrackSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Student)
            .WithMany(s => s.Attendances)
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder.ToTable("Assignments", a =>
        {
            a.HasCheckConstraint("CK_Assignments_MaxScore", "[MaxScore] > 0.00");
            a.HasCheckConstraint("CK_Assignments_Weight", "[WeightPercentage] >= 0.00 AND [WeightPercentage] <= 100.00");
        });

        builder.HasKey(a => a.AssignmentId);
        builder.Property(a => a.AssignmentId).UseIdentityColumn();

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(a => a.Description)
            .HasMaxLength(1000);

        builder.Property(a => a.MaxScore)
            .IsRequired()
            .HasPrecision(5, 2)
            .HasDefaultValue(100.00m);

        builder.Property(a => a.WeightPercentage)
            .IsRequired()
            .HasPrecision(5, 2)
            .HasDefaultValue(10.00m);

        builder.Property(a => a.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasIndex(a => a.TrainingTrackId)
            .HasDatabaseName("IX_Assignments_TrackId");

        builder.HasOne(a => a.TrainingTrack)
            .WithMany(t => t.Assignments)
            .HasForeignKey(a => a.TrainingTrackId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(a => a.Submissions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class AssignmentSubmissionConfiguration : IEntityTypeConfiguration<AssignmentSubmission>
{
    public void Configure(EntityTypeBuilder<AssignmentSubmission> builder)
    {
        builder.ToTable("AssignmentSubmissions");

        builder.HasKey(s => s.SubmissionId);
        builder.Property(s => s.SubmissionId).UseIdentityColumn();

        builder.Property(s => s.ContentUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.Score)
            .HasPrecision(5, 2);

        builder.Property(s => s.Feedback)
            .HasMaxLength(1000);

        builder.Property(s => s.SubmittedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasIndex(s => new { s.AssignmentId, s.StudentId })
            .IsUnique()
            .HasDatabaseName("UQ_Submissions_Assignment_Student");

        builder.HasIndex(s => s.StudentId)
            .HasDatabaseName("IX_AssignmentSubmissions_StudentId");

        builder.HasOne(s => s.Assignment)
            .WithMany(a => a.Submissions)
            .HasForeignKey(s => s.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Student)
            .WithMany(st => st.Submissions)
            .HasForeignKey(s => s.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
