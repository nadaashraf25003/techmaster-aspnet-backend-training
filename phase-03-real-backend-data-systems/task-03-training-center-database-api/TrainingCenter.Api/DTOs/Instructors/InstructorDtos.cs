using System.ComponentModel.DataAnnotations;

namespace TrainingCenter.Api.DTOs.Instructors;

public class CreateInstructorRequest
{
    [Required(ErrorMessage = "Full Name is required.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Full Name must be between 2 and 150 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Specialization is required.")]
    [StringLength(100, ErrorMessage = "Specialization cannot exceed 100 characters.")]
    public string Specialization { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Bio cannot exceed 1000 characters.")]
    public string? Bio { get; set; }

    public bool IsActive { get; set; } = true;
}

public class UpdateInstructorRequest
{
    [Required(ErrorMessage = "Full Name is required.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Full Name must be between 2 and 150 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Specialization is required.")]
    [StringLength(100, ErrorMessage = "Specialization cannot exceed 100 characters.")]
    public string Specialization { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Bio cannot exceed 1000 characters.")]
    public string? Bio { get; set; }

    public bool IsActive { get; set; } = true;
}

public class InstructorResponse
{
    public int InstructorId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public bool IsActive { get; set; }
    public int ActiveTracksCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class InstructorDetailsResponse
{
    public int InstructorId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<InstructorTrackSummaryDto> Tracks { get; set; } = new();
}

public class InstructorTrackSummaryDto
{
    public int TrainingTrackId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int EnrolledStudentsCount { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
