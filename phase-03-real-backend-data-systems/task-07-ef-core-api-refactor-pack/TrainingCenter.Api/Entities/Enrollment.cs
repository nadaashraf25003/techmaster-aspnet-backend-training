using TrainingCenter.Api.Common;

namespace TrainingCenter.Api.Entities;

public class Enrollment : BaseAuditableEntity
{
    private readonly List<Payment> _payments = new();

    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public int TrainingTrackId { get; set; }
    public TrainingTrack? TrainingTrack { get; set; }
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;

    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();
}
