using System.ComponentModel.DataAnnotations;
using TrainingCenter.Api.Common;
using TrainingCenter.Api.DTOs.Common;

namespace TrainingCenter.Api.DTOs.Enrollments;

public class CreateEnrollmentRequest
{
    [Required(ErrorMessage = "StudentId is required.")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "TrainingTrackId is required.")]
    public int TrainingTrackId { get; set; }

    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
}

public class UpdateEnrollmentStatusRequest
{
    [Required(ErrorMessage = "Enrollment status is required.")]
    public EnrollmentStatus Status { get; set; }

    [Range(0, 100, ErrorMessage = "Progress percentage must be between 0 and 100.")]
    public decimal? ProgressPercentage { get; set; }

    public string? FinalResult { get; set; }
}

public class EnrollmentFilterParams : PaginationParams
{
    public EnrollmentStatus? Status { get; set; }
    public int? TrackId { get; set; }
    public int? StudentId { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
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
    public DateTime EnrollmentDate { get; set; }
    public EnrollmentStatus Status { get; set; }
    public decimal ProgressPercentage { get; set; }
    public string? FinalResult { get; set; }
    public decimal TrackPrice { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal OutstandingBalance => Math.Max(0, TrackPrice - TotalPaid);
    public bool IsFullyPaid => TotalPaid >= TrackPrice;
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
    public DateTime EnrollmentDate { get; set; }
    public EnrollmentStatus Status { get; set; }
    public decimal ProgressPercentage { get; set; }
    public string? FinalResult { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal OutstandingBalance => Math.Max(0, TrackPrice - TotalPaid);
    public bool IsFullyPaid => TotalPaid >= TrackPrice;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<EnrollmentPaymentSummaryDto> Payments { get; set; } = new();
}

public class EnrollmentPaymentSummaryDto
{
    public int PaymentId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
