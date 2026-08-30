using System.ComponentModel.DataAnnotations;

namespace StudentManagementApi.DTOs;

/// <summary>
/// Request DTO for updating a student's active status.
/// </summary>
public class UpdateStudentStatusDto
{
    [Required(ErrorMessage = "IsActive status is required.")]
    public bool IsActive { get; set; }
}
