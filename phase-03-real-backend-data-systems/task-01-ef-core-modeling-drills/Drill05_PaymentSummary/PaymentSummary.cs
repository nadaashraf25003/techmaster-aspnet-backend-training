using EFCoreModelingDrills.Common;
using EFCoreModelingDrills.Drill04_ManyToManyEnrollment;

namespace EFCoreModelingDrills.Drill05_PaymentSummary;

/// <summary>
/// Drill 05: One-to-One PaymentSummary Entity.
/// Linked to an Enrollment to track financial obligations, payments, and remaining balance.
/// </summary>
public class PaymentSummary : BaseAuditableEntity
{
    public int Id { get; set; }

    // 1:1 Unique Foreign Key to Enrollment
    public int EnrollmentId { get; set; }
    public Enrollment? Enrollment { get; set; }

    // Financial Metrics with strict decimal precision
    public decimal TotalRequired { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal RemainingAmount => Math.Max(0, TotalRequired - TotalPaid);
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    public void RecordPayment(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Payment amount must be greater than zero.", nameof(amount));

        TotalPaid += amount;
        PaymentStatus = TotalPaid >= TotalRequired 
            ? PaymentStatus.Paid 
            : PaymentStatus.PartiallyPaid;
    }
}
