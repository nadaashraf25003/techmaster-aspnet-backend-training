using System.ComponentModel.DataAnnotations;
using TrainingCenter.Api.DTOs.Common;

namespace TrainingCenter.Api.DTOs.Instructors;

public class InstructorFilterParams : PaginationParams
{
    public string? Search { get; set; }
    public string? Specialization { get; set; }
    public bool? IsActive { get; set; }
}

public class InstructorResponse
{
    public int InstructorId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public bool IsActive { get; set; }
    public int AssignedTracksCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateInstructorRequest
{
    [Required(ErrorMessage = "FullName is required.")]
    [MaxLength(150, ErrorMessage = "FullName cannot exceed 150 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [MaxLength(200, ErrorMessage = "Email cannot exceed 200 characters.")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number.")]
    [MaxLength(30, ErrorMessage = "PhoneNumber cannot exceed 30 characters.")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Specialization is required.")]
    [MaxLength(100, ErrorMessage = "Specialization cannot exceed 100 characters.")]
    public string Specialization { get; set; } = string.Empty;

    [Required(ErrorMessage = "HourlyRate is required.")]
    [Range(0, 10000.00, ErrorMessage = "HourlyRate must be between 0 and 10,000.00.")]
    public decimal HourlyRate { get; set; }
}

public class UpdateInstructorRequest
{
    [Required(ErrorMessage = "FullName is required.")]
    [MaxLength(150, ErrorMessage = "FullName cannot exceed 150 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [MaxLength(200, ErrorMessage = "Email cannot exceed 200 characters.")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number.")]
    [MaxLength(30, ErrorMessage = "PhoneNumber cannot exceed 30 characters.")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Specialization is required.")]
    [MaxLength(100, ErrorMessage = "Specialization cannot exceed 100 characters.")]
    public string Specialization { get; set; } = string.Empty;

    [Required(ErrorMessage = "HourlyRate is required.")]
    [Range(0, 10000.00, ErrorMessage = "HourlyRate must be between 0 and 10,000.00.")]
    public decimal HourlyRate { get; set; }

    public bool IsActive { get; set; } = true;
}
