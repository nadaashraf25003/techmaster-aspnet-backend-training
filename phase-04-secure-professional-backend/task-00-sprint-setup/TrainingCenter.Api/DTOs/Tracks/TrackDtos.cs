using System.ComponentModel.DataAnnotations;
using TrainingCenter.Api.Common;
using TrainingCenter.Api.DTOs.Common;

namespace TrainingCenter.Api.DTOs.Tracks;

public class TrackFilterParams : PaginationParams
{
    public string? Search { get; set; }
    public TrackStatus? Status { get; set; }
    public int? InstructorId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}

public class TrackResponse
{
    public int TrainingTrackId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationHours { get; set; }
    public int Capacity { get; set; }
    public TrackStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? PrimaryInstructorId { get; set; }
    public string? InstructorName { get; set; }
    public int ActiveEnrollmentsCount { get; set; }
    public int AvailableSeats => Math.Max(0, Capacity - ActiveEnrollmentsCount);
    public bool IsFull => AvailableSeats <= 0;
    public DateTime CreatedAt { get; set; }
}

public class CreateTrackRequest
{
    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Code is required.")]
    [MaxLength(50, ErrorMessage = "Code cannot exceed 50 characters.")]
    public string Code { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required.")]
    [Range(0, 100000.00, ErrorMessage = "Price must be between 0 and 100,000.00.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "DurationHours is required.")]
    [Range(1, 1000, ErrorMessage = "DurationHours must be between 1 and 1000.")]
    public int DurationHours { get; set; }

    // Rule T2: Capacity must be strictly greater than 0
    [Required(ErrorMessage = "Capacity is required.")]
    [Range(1, 500, ErrorMessage = "Capacity must be greater than 0.")]
    public int Capacity { get; set; }

    public TrackStatus Status { get; set; } = TrackStatus.Upcoming;

    // Rule T3: StartDate < EndDate
    [Required(ErrorMessage = "StartDate is required.")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "EndDate is required.")]
    public DateTime EndDate { get; set; }

    // Rule T4: Instructor is required
    [Required(ErrorMessage = "Instructor is required.")]
    public int? PrimaryInstructorId { get; set; }
}

public class UpdateTrackRequest
{
    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required.")]
    [Range(0, 100000.00, ErrorMessage = "Price must be between 0 and 100,000.00.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "DurationHours is required.")]
    [Range(1, 1000, ErrorMessage = "DurationHours must be between 1 and 1000.")]
    public int DurationHours { get; set; }

    // Rule T2: Capacity must be strictly greater than 0
    [Required(ErrorMessage = "Capacity is required.")]
    [Range(1, 500, ErrorMessage = "Capacity must be greater than 0.")]
    public int Capacity { get; set; }

    public TrackStatus Status { get; set; }

    // Rule T3: StartDate < EndDate
    [Required(ErrorMessage = "StartDate is required.")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "EndDate is required.")]
    public DateTime EndDate { get; set; }

    // Rule T4: Instructor is required
    [Required(ErrorMessage = "Instructor is required.")]
    public int? PrimaryInstructorId { get; set; }
}
