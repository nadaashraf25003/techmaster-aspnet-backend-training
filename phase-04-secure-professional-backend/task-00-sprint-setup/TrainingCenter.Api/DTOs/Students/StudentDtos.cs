using System.ComponentModel.DataAnnotations;
using TrainingCenter.Api.DTOs.Common;

namespace TrainingCenter.Api.DTOs.Students;

public class StudentFilterParams : PaginationParams
{
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
}

public class StudentResponse
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
    public int EnrollmentsCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateStudentRequest
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

    [Required(ErrorMessage = "DateOfBirth is required.")]
    public DateTime DateOfBirth { get; set; }

    [MaxLength(300, ErrorMessage = "Address cannot exceed 300 characters.")]
    public string? Address { get; set; }
}

public class UpdateStudentRequest
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

    [Required(ErrorMessage = "DateOfBirth is required.")]
    public DateTime DateOfBirth { get; set; }

    [MaxLength(300, ErrorMessage = "Address cannot exceed 300 characters.")]
    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;
}
