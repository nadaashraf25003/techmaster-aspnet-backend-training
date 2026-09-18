using TechMaster.Phase03.Task02.Common;

namespace TechMaster.Phase03.Task02.Entities;

public class Payment
{
    public int PaymentId { get; set; }

    // Foreign Key
    public int EnrollmentId { get; set; }
    public Enrollment? Enrollment { get; set; }

    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string ReferenceNumber { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
