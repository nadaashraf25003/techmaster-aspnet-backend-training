using System.ComponentModel.DataAnnotations;
using TrainingCenter.Api.Common;

namespace TrainingCenter.Api.DTOs.Payments;

public class ProcessPaymentRequestDto
{
    [Required(ErrorMessage = "EnrollmentId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "EnrollmentId must be greater than 0")]
    public int EnrollmentId { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, 1000000.00, ErrorMessage = "Payment amount must be greater than 0")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "PaymentMethod is required")]
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CreditCard;

    [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
}

public class PaymentResponseDto
{
    public int Id { get; set; }
    public int EnrollmentId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus Status { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
