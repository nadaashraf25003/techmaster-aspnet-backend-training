using System.ComponentModel.DataAnnotations;
using TrainingCenter.Api.Common;
using TrainingCenter.Api.DTOs.Common;

namespace TrainingCenter.Api.DTOs.Tracks;

public class CreateTrackRequest
{
    [Required(ErrorMessage = "Track Title is required.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Track Code is required.")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "Track Code must be between 2 and 30 characters.")]
    public string Code { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Level is required.")]
    public TrackLevel Level { get; set; } = TrackLevel.Beginner;

    [Range(0, 100000, ErrorMessage = "Price must be greater than or equal to 0.")]
    public decimal Price { get; set; } = 0.00m;

    [Range(1, 1000, ErrorMessage = "Capacity must be at least 1.")]
    public int Capacity { get; set; } = 20;

    [Required(ErrorMessage = "Start Date is required.")]
    public DateOnly StartDate { get; set; }

    [Required(ErrorMessage = "End Date is required.")]
    public DateOnly EndDate { get; set; }

    public TrackStatus Status { get; set; } = TrackStatus.Draft;

    [Required(ErrorMessage = "InstructorId is required.")]
    public int InstructorId { get; set; }
}

public class UpdateTrackRequest
{
    [Required(ErrorMessage = "Track Title is required.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Track Code is required.")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "Track Code must be between 2 and 30 characters.")]
    public string Code { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Level is required.")]
    public TrackLevel Level { get; set; }

    [Range(0, 100000, ErrorMessage = "Price must be greater than or equal to 0.")]
    public decimal Price { get; set; }

    [Range(1, 1000, ErrorMessage = "Capacity must be at least 1.")]
    public int Capacity { get; set; }

    [Required(ErrorMessage = "Start Date is required.")]
    public DateOnly StartDate { get; set; }

    [Required(ErrorMessage = "End Date is required.")]
    public DateOnly EndDate { get; set; }

    public TrackStatus Status { get; set; }

    [Required(ErrorMessage = "InstructorId is required.")]
    public int InstructorId { get; set; }
}

public class TrackFilterParams : PaginationParams
{
    public string? Keyword { get; set; }
    public TrackLevel? Level { get; set; }
    public TrackStatus? Status { get; set; }
    public int? InstructorId { get; set; }
}

public class TrackListItemResponse
{
    public int TrainingTrackId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public TrackLevel Level { get; set; }
    public decimal Price { get; set; }
    public int Capacity { get; set; }
    public int EnrolledStudentsCount { get; set; }
    public int AvailableSeats => Math.Max(0, Capacity - EnrolledStudentsCount);
    public bool IsFull => EnrolledStudentsCount >= Capacity;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public TrackStatus Status { get; set; }
    public int InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
}

public class TrackDetailsResponse
{
    public int TrainingTrackId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TrackLevel Level { get; set; }
    public decimal Price { get; set; }
    public int Capacity { get; set; }
    public int EnrolledStudentsCount { get; set; }
    public int AvailableSeats => Math.Max(0, Capacity - EnrolledStudentsCount);
    public decimal OccupancyRate => Capacity > 0 ? Math.Round((EnrolledStudentsCount / (decimal)Capacity) * 100, 2) : 0;
    public bool IsFull => EnrolledStudentsCount >= Capacity;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public TrackStatus Status { get; set; }
    public int InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public string InstructorEmail { get; set; } = string.Empty;
    public List<TrackStudentDto> EnrolledStudents { get; set; } = new();
}

public class TrackStudentDto
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public int EnrollmentId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public EnrollmentStatus Status { get; set; }
    public decimal ProgressPercentage { get; set; }
}
