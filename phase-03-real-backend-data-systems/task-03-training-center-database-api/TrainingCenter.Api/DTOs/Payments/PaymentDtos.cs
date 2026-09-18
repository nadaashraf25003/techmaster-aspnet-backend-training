using System.ComponentModel.DataAnnotations;
using TrainingCenter.Api.Common;
using TrainingCenter.Api.DTOs.Common;

namespace TrainingCenter.Api.DTOs.Payments;

public class CreatePaymentRequest
{
    [Required(ErrorMessage = "EnrollmentId is required.")]
    public int EnrollmentId { get; set; }

    [Required(ErrorMessage = "Amount is required.")]
    [Range(0.01, 1000000.00, ErrorMessage = "Payment amount must be greater than 0.")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Payment Method is required.")]
    public PaymentMethod PaymentMethod { get; set; }

    [Required(ErrorMessage = "Reference Number is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Reference Number must be between 3 and 100 characters.")]
    public string ReferenceNumber { get; set; } = string.Empty;

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Completed;

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
    public string? Notes { get; set; }
}

public class UpdatePaymentStatusRequest
{
    [Required(ErrorMessage = "Payment status is required.")]
    public PaymentStatus Status { get; set; }

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
    public string? Notes { get; set; }
}

public class PaymentFilterParams : PaginationParams
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public PaymentStatus? Status { get; set; }
    public PaymentMethod? Method { get; set; }
    public int? EnrollmentId { get; set; }
}

public class PaymentResponse
{
    public int PaymentId { get; set; }
    public int EnrollmentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string TrackTitle { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
