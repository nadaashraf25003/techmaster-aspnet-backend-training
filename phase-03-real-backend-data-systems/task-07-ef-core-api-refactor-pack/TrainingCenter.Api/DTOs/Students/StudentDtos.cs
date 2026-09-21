using System.ComponentModel.DataAnnotations;

namespace TrainingCenter.Api.DTOs.Students;

public class CreateStudentRequestDto
{
    [Required(ErrorMessage = "FullName is required")]
    [MaxLength(150, ErrorMessage = "FullName cannot exceed 150 characters")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [MaxLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number format")]
    [MaxLength(20, ErrorMessage = "PhoneNumber cannot exceed 20 characters")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "DateOfBirth is required")]
    public DateTime DateOfBirth { get; set; }

    [MaxLength(300, ErrorMessage = "Address cannot exceed 300 characters")]
    public string? Address { get; set; }
}

public class UpdateStudentRequestDto
{
    [Required(ErrorMessage = "FullName is required")]
    [MaxLength(150, ErrorMessage = "FullName cannot exceed 150 characters")]
    public string FullName { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number format")]
    [MaxLength(20, ErrorMessage = "PhoneNumber cannot exceed 20 characters")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "DateOfBirth is required")]
    public DateTime DateOfBirth { get; set; }

    [MaxLength(300, ErrorMessage = "Address cannot exceed 300 characters")]
    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;
}

public class StudentResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
    public int EnrollmentsCount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
