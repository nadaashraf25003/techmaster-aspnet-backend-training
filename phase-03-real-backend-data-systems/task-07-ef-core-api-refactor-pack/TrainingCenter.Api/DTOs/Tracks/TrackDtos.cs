using System.ComponentModel.DataAnnotations;
using TrainingCenter.Api.Common;

namespace TrainingCenter.Api.DTOs.Tracks;

public class CreateTrackRequestDto
{
    [Required(ErrorMessage = "Code is required")]
    [MaxLength(20, ErrorMessage = "Code cannot exceed 20 characters")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(150, ErrorMessage = "Name cannot exceed 150 characters")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0, 1000000.00, ErrorMessage = "Price must be non-negative")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Capacity is required")]
    [Range(1, 1000, ErrorMessage = "Capacity must be at least 1")]
    public int Capacity { get; set; }

    public TrackStatus Status { get; set; } = TrackStatus.Planned;
    public int? InstructorId { get; set; }
}

public class TrackResponseDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Capacity { get; set; }
    public TrackStatus Status { get; set; }
    public int? InstructorId { get; set; }
    public string? InstructorName { get; set; }
    public int ActiveEnrollmentsCount { get; set; }
    public int RemainingSeats => Math.Max(0, Capacity - ActiveEnrollmentsCount);
    public bool IsFull => RemainingSeats == 0;
    public DateTime CreatedAtUtc { get; set; }
}
