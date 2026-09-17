using EFCoreModelingDrills.Common;
using EFCoreModelingDrills.Drill01_DbContextFirstMigration;
using EFCoreModelingDrills.Drill03_OneToManyInstructorTracks;
using EFCoreModelingDrills.Drill05_PaymentSummary;

namespace EFCoreModelingDrills.Drill04_ManyToManyEnrollment;

/// <summary>
/// Drill 04: Enrollment Join Entity.
/// Represents a rich Many-to-Many relationship between Student and TrainingTrack
/// carrying domain payloads (Status, ProgressPercentage, FinalGrade, EnrollmentDate).
/// </summary>
public class Enrollment : BaseAuditableEntity
{
    public int Id { get; set; }

    // Foreign Keys
    public int StudentId { get; set; }
    public Student? Student { get; set; }

    public int TrainingTrackId { get; set; }
    public TrainingTrack? TrainingTrack { get; set; }

    // Payload Fields
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending;
    public int ProgressPercentage { get; set; } = 0;
    public decimal? FinalGrade { get; set; }

    // Drill 05: 1:1 Navigation to PaymentSummary
    public PaymentSummary? PaymentSummary { get; set; }
}
