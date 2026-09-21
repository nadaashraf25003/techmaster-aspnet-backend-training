using TrainingCenter.Api.Common;

namespace TrainingCenter.Api.Entities;

public class Payment : BaseAuditableEntity
{
    public int Id { get; set; }
    public int EnrollmentId { get; set; }
    public Enrollment? Enrollment { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public PaymentStatus Status { get; set; } = PaymentStatus.Paid;
    public string ReferenceNumber { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
