using System.ComponentModel.DataAnnotations;

namespace StudentManagementApi.DTOs;

/// <summary>
/// Request DTO for updating an existing student profile.
/// </summary>
public class UpdateStudentDto
{
    [Required(ErrorMessage = "FullName is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "FullName must be between 2 and 100 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "PhoneNumber is required.")]
    [Phone(ErrorMessage = "PhoneNumber must be a valid phone number format.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "TrackName is required.")]
    public string TrackName { get; set; } = string.Empty;

    [Required(ErrorMessage = "IsActive is required.")]
    public bool IsActive { get; set; }
}
