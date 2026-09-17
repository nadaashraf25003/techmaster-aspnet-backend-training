using EFCoreModelingDrills.Common;

namespace EFCoreModelingDrills.Drill09_ProjectionDTO;

/// <summary>
/// Drill 09: Lightweight DTO for Student list items, avoiding entity exposure and cyclic references.
/// </summary>
public class StudentListItemDto
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public int ActiveEnrollmentsCount { get; set; }
}

/// <summary>
/// Drill 09: Detailed Track DTO containing instructor name and capacity statistics.
/// </summary>
public class TrackDetailsDto
{
    public int TrackId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public TrackLevel Level { get; set; }
    public int Capacity { get; set; }
    public int EnrolledStudentsCount { get; set; }
    public int AvailableSeats => Math.Max(0, Capacity - EnrolledStudentsCount);
    public decimal Price { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public string InstructorEmail { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
