using System.ComponentModel.DataAnnotations;
using TrainingCenter.Api.Common;
using TrainingCenter.Api.DTOs.Common;

namespace TrainingCenter.Api.DTOs.Students;

public class CreateStudentRequest
{
    [Required(ErrorMessage = "Full Name is required.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Full Name must be between 2 and 150 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number format.")]
    [StringLength(25, ErrorMessage = "Phone number cannot exceed 25 characters.")]
    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; } = true;
}

public class UpdateStudentRequest
{
    [Required(ErrorMessage = "Full Name is required.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Full Name must be between 2 and 150 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number format.")]
    [StringLength(25, ErrorMessage = "Phone number cannot exceed 25 characters.")]
    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; } = true;
}

public class StudentFilterParams : PaginationParams
{
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
}

public class StudentListItemResponse
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public int ActiveEnrollmentsCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class StudentDetailsResponse
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<StudentEnrollmentSummaryDto> Enrollments { get; set; } = new();
}

public class StudentEnrollmentSummaryDto
{
    public int EnrollmentId { get; set; }
    public int TrainingTrackId { get; set; }
    public string TrackCode { get; set; } = string.Empty;
    public string TrackTitle { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public EnrollmentStatus Status { get; set; }
    public decimal ProgressPercentage { get; set; }
    public string? FinalResult { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TrackPrice { get; set; }
}
