using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.Common;
using TrainingCenter.Api.DTOs.Common;

namespace TrainingCenter.Api.DTOs.Enrollments;

public class EnrollmentFilterParams : PaginationParams
{
    [FromQuery(Name = "trackId")]
    public int? TrackId { get; set; }

    [FromQuery(Name = "studentId")]
    public int? StudentId { get; set; }

    [FromQuery(Name = "status")]
    public EnrollmentStatus? Status { get; set; }

    [FromQuery(Name = "paymentStatus")]
    public string? PaymentStatus { get; set; }

    [FromQuery(Name = "from")]
    public DateTime? From { get; set; }

    [FromQuery(Name = "to")]
    public DateTime? To { get; set; }

    [FromQuery(Name = "search")]
    public string? Search { get; set; }
}

public class EnrollmentListItemResponse
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public int TrainingTrackId { get; set; }
    public string TrackCode { get; set; } = string.Empty;
    public string TrackTitle { get; set; } = string.Empty;
    public decimal TrackPrice { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public EnrollmentStatus Status { get; set; }
    public decimal ProgressPercentage { get; set; }
    public string? FinalResult { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal OutstandingBalance => TrackPrice - TotalPaid;
    public string FinancialStatus => TotalPaid >= TrackPrice ? "Fully Paid" : TotalPaid > 0 ? "Partially Paid" : "Unpaid";
}

public class EnrollmentDetailsResponse
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public string? StudentPhone { get; set; }
    public int TrainingTrackId { get; set; }
    public string TrackCode { get; set; } = string.Empty;
    public string TrackTitle { get; set; } = string.Empty;
    public decimal TrackPrice { get; set; }
    public DateTime TrackStartDate { get; set; }
    public DateTime TrackEndDate { get; set; }
    public string? InstructorName { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public EnrollmentStatus Status { get; set; }
    public decimal ProgressPercentage { get; set; }
    public string? FinalResult { get; set; }
    public string? Notes { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal OutstandingBalance => TrackPrice - TotalPaid;
    public List<EnrollmentPaymentSummaryResponse> Payments { get; set; } = new();
}

public class EnrollmentPaymentSummaryResponse
{
    public int PaymentId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public DateTime PaymentDate { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
}

public class CreateEnrollmentRequest
{
    [Required(ErrorMessage = "StudentId is required.")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "TrainingTrackId is required.")]
    public int TrainingTrackId { get; set; }

    [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
    public string? Notes { get; set; }
}

public class UpdateEnrollmentStatusRequest
{
    [Required(ErrorMessage = "Status is required.")]
    public EnrollmentStatus Status { get; set; }

    [Range(0, 100, ErrorMessage = "ProgressPercentage must be between 0 and 100.")]
    public decimal? ProgressPercentage { get; set; }

    [MaxLength(50, ErrorMessage = "FinalResult cannot exceed 50 characters.")]
    public string? FinalResult { get; set; }

    [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
    public string? Notes { get; set; }
}
