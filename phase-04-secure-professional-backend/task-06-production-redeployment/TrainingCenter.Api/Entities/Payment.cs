using TrainingCenter.Api.Common;

namespace TrainingCenter.Api.Entities;

public class Payment : BaseAuditableEntity
{
    public int PaymentId { get; set; }
    public int EnrollmentId { get; set; }
    public virtual Enrollment? Enrollment { get; set; }

    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CreditCard;
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Completed;
    public string ReferenceNumber { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
