using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.Common;
using TrainingCenter.Api.DTOs.Common;

namespace TrainingCenter.Api.DTOs.Payments;

public class PaymentFilterParams : PaginationParams
{
    /// <summary>
    /// Start date for filtering payments (Query 13 parameter: from=YYYY-MM-DD).
    /// </summary>
    [FromQuery(Name = "from")]
    public DateTime? From { get; set; }

    /// <summary>
    /// End date for filtering payments (Query 13 parameter: to=YYYY-MM-DD).
    /// </summary>
    [FromQuery(Name = "to")]
    public DateTime? To { get; set; }

    /// <summary>
    /// Start date alias (startDate=YYYY-MM-DD).
    /// </summary>
    [FromQuery(Name = "startDate")]
    public DateTime? StartDate
    {
        get => From;
        set => From = value;
    }

    /// <summary>
    /// End date alias (endDate=YYYY-MM-DD).
    /// </summary>
    [FromQuery(Name = "endDate")]
    public DateTime? EndDate
    {
        get => To;
        set => To = value;
    }

    /// <summary>
    /// Filter by payment status (e.g., Completed, Pending, Failed).
    /// </summary>
    public PaymentStatus? Status { get; set; }

    /// <summary>
    /// Filter by payment method (e.g., CreditCard, BankTransfer, Fawry, VodafoneCash).
    /// </summary>
    public PaymentMethod? Method { get; set; }

    /// <summary>
    /// Filter by specific enrollment ID.
    /// </summary>
    public int? EnrollmentId { get; set; }

    /// <summary>
    /// Validates if date range is valid (from &lt;= to).
    /// </summary>
    public bool IsDateRangeValid => !(From.HasValue && To.HasValue && From.Value > To.Value);
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

public class CreatePaymentRequest
{
    [Required(ErrorMessage = "EnrollmentId is required.")]
    public int EnrollmentId { get; set; }

    [Required(ErrorMessage = "Amount is required.")]
    [Range(0.01, 100000.00, ErrorMessage = "Amount must be between 0.01 and 100,000.00.")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "PaymentMethod is required.")]
    public PaymentMethod PaymentMethod { get; set; }

    [Required(ErrorMessage = "PaymentDate is required.")]
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    [MaxLength(100, ErrorMessage = "ReferenceNumber cannot exceed 100 characters.")]
    public string? ReferenceNumber { get; set; }

    [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
    public string? Notes { get; set; }
}

public class UpdatePaymentStatusRequest
{
    [Required(ErrorMessage = "PaymentStatus is required.")]
    public PaymentStatus Status { get; set; }

    [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
    public string? Notes { get; set; }
}
