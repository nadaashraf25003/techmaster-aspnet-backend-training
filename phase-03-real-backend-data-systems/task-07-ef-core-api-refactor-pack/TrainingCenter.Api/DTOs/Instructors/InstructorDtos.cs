using System.ComponentModel.DataAnnotations;

namespace TrainingCenter.Api.DTOs.Instructors;

public class CreateInstructorRequestDto
{
    [Required(ErrorMessage = "FullName is required")]
    [MaxLength(150, ErrorMessage = "FullName cannot exceed 150 characters")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone format")]
    [MaxLength(20, ErrorMessage = "PhoneNumber cannot exceed 20 characters")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Specialization is required")]
    [MaxLength(100, ErrorMessage = "Specialization cannot exceed 100 characters")]
    public string Specialization { get; set; } = string.Empty;

    [Required(ErrorMessage = "HourlyRate is required")]
    [Range(0, 100000.00, ErrorMessage = "HourlyRate must be non-negative")]
    public decimal HourlyRate { get; set; }
}

public class InstructorResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public bool IsActive { get; set; }
    public int AssignedTracksCount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
