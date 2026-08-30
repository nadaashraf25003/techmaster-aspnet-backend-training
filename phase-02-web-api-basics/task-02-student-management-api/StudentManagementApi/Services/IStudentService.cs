using StudentManagementApi.DTOs;

namespace StudentManagementApi.Services;

/// <summary>
/// Interface for student business logic and operations.
/// </summary>
public interface IStudentService
{
    /// <summary>
    /// Retrieves a paginated list of students based on search, filter, and pagination parameters.
    /// </summary>
    Task<PagedResponse<StudentResponseDto>> GetAllStudentsAsync(StudentQueryParametersDto query);

    /// <summary>
    /// Retrieves a student by their unique identifier.
    /// </summary>
    Task<StudentResponseDto?> GetStudentByIdAsync(int id);

    /// <summary>
    /// Creates a new student profile after validating business rules (e.g., unique email).
    /// </summary>
    Task<(bool Success, string? ErrorMessage, StudentResponseDto? Student)> CreateStudentAsync(CreateStudentDto dto);

    /// <summary>
    /// Updates all modifiable fields of an existing student.
    /// </summary>
    Task<(bool Success, string? ErrorMessage, StudentResponseDto? Student)> UpdateStudentAsync(int id, UpdateStudentDto dto);

    /// <summary>
    /// Updates only the active status of an existing student.
    /// </summary>
    Task<(bool Success, string? ErrorMessage, StudentResponseDto? Student)> UpdateStudentStatusAsync(int id, bool isActive);

    /// <summary>
    /// Calculates aggregate statistics across all students.
    /// </summary>
    Task<StudentStatsDto> GetStudentStatsAsync();
}
