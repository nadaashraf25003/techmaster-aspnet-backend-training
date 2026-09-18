using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Students;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Controllers;

/// <summary>
/// Manages student profiles with strict business validation (unique email, soft delete safeguard, etc.).
/// </summary>
public class StudentsController : BaseApiController
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    /// <summary>
    /// Retrieves a paginated and filtered list of active (non-deleted) students.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<StudentResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudents([FromQuery] StudentFilterParams filters)
    {
        var result = await _studentService.GetStudentsAsync(filters);
        return Success(result, "Students retrieved successfully");
    }

    /// <summary>
    /// Retrieves student details by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentById(int id)
    {
        var result = await _studentService.GetStudentByIdAsync(id);
        return Success(result, "Student details retrieved successfully");
    }

    /// <summary>
    /// Creates a new student. Validates email uniqueness and required fields.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentRequest request)
    {
        var result = await _studentService.CreateStudentAsync(request);
        return CreatedSuccess(nameof(GetStudentById), new { id = result.StudentId }, result, "Student registered successfully");
    }

    /// <summary>
    /// Updates an existing student's profile.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentRequest request)
    {
        var result = await _studentService.UpdateStudentAsync(id, request);
        return Success(result, "Student profile updated successfully");
    }

    /// <summary>
    /// Soft-deletes a student and archives their active enrollments safely.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        await _studentService.DeleteStudentAsync(id);
        return Success<object?>(null, "Student soft-deleted successfully");
    }
}
