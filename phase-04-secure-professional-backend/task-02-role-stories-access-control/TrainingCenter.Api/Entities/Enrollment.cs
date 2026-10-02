using TrainingCenter.Api.Common;

namespace TrainingCenter.Api.Entities;

public class Enrollment : BaseAuditableEntity
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public virtual Student? Student { get; set; }

    public int TrainingTrackId { get; set; }
    public virtual TrainingTrack? TrainingTrack { get; set; }

    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending;
    public decimal ProgressPercentage { get; set; } = 0.00m;
    public string? FinalResult { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    private readonly List<Payment> _payments = new();
    public virtual IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    public void AddPayment(Payment payment)
    {
        _payments.Add(payment);
    }
}
