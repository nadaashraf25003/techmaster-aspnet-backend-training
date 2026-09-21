using System.ComponentModel.DataAnnotations;
using TrainingCenter.Api.Common;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Payments;

namespace TrainingCenter.Api.DTOs.Enrollments;

public class CreateEnrollmentRequestDto
{
    [Required(ErrorMessage = "StudentId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "StudentId must be greater than 0")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "TrainingTrackId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "TrainingTrackId must be greater than 0")]
    public int TrainingTrackId { get; set; }
}

public class EnrollmentResponseDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public int TrainingTrackId { get; set; }
    public string TrackCode { get; set; } = string.Empty;
    public string TrackName { get; set; } = string.Empty;
    public decimal TrackPrice { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public EnrollmentStatus Status { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public decimal RemainingBalance => Math.Max(0, TrackPrice - TotalPaidAmount);
    public bool IsFullyPaid => RemainingBalance <= 0;
}

public class EnrollmentDetailResponseDto : EnrollmentResponseDto
{
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public List<PaymentResponseDto> Payments { get; set; } = new();
}

public class EnrollmentQueryParameters : PaginationParams
{
    public int? StudentId { get; set; }
    public int? TrackId { get; set; }
    public EnrollmentStatus? Status { get; set; }
    public bool? IsFullyPaid { get; set; }
    public string? SearchTerm { get; set; }
}
