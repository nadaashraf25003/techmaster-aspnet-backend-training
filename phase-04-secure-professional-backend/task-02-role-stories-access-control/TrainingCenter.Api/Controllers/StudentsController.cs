using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Enrollments;
using TrainingCenter.Api.DTOs.Payments;
using TrainingCenter.Api.DTOs.Students;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Controllers;

/// <summary>
/// Manages student profiles and student self-service portal operations.
/// </summary>
public class StudentsController : BaseApiController
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    /// <summary>
    /// Retrieves a paginated and filtered list of students. Restricted to Administrators.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<StudentResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetStudents([FromQuery] StudentFilterParams filters)
    {
        var result = await _studentService.GetStudentsAsync(filters);
        return Success(result, "Students retrieved successfully");
    }

    /// <summary>
    /// Retrieves student details by ID. Admins can view any student; Students can only view their own profile.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentById(int id)
    {
        if (!IsAdmin())
        {
            var loggedInStudentId = GetAuthenticatedStudentId();
            if (!loggedInStudentId.HasValue || loggedInStudentId.Value != id)
            {
                throw new ForbiddenException("Access denied. You can only view your own student profile.");
            }
        }

        var result = await _studentService.GetStudentByIdAsync(id);
        return Success(result, "Student details retrieved successfully");
    }

    /// <summary>
    /// Student Portal Story: Retrieves the currently logged-in student's profile.
    /// Accessible via GET /api/students/me and GET /api/student/my-profile.
    /// </summary>
    [HttpGet("me")]
    [HttpGet("/api/student/my-profile")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMyProfile()
    {
        var studentId = GetAuthenticatedStudentId();
        if (!studentId.HasValue)
        {
            throw new ForbiddenException("No student profile linked to the current user.");
        }

        var result = await _studentService.GetStudentByIdAsync(studentId.Value);
        return Success(result, "Student profile retrieved successfully");
    }

    /// <summary>
    /// Student Portal Story: Retrieves all enrollments belonging to the logged-in student.
    /// Accessible via GET /api/students/me/enrollments and GET /api/student/my-enrollments.
    /// </summary>
    [HttpGet("me/enrollments")]
    [HttpGet("/api/student/my-enrollments")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(ApiResponse<List<EnrollmentListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMyEnrollments()
    {
        var studentId = GetAuthenticatedStudentId();
        if (!studentId.HasValue)
        {
            throw new ForbiddenException("No student profile linked to the current user.");
        }

        var result = await _studentService.GetStudentEnrollmentsAsync(studentId.Value);
        return Success(result, "Student enrollments retrieved successfully");
    }

    /// <summary>
    /// Student Portal Story: Retrieves payment transaction history for the logged-in student.
    /// Accessible via GET /api/students/me/payments and GET /api/student/my-payments.
    /// </summary>
    [HttpGet("me/payments")]
    [HttpGet("/api/student/my-payments")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(ApiResponse<List<PaymentResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMyPayments()
    {
        var studentId = GetAuthenticatedStudentId();
        if (!studentId.HasValue)
        {
            throw new ForbiddenException("No student profile linked to the current user.");
        }

        var result = await _studentService.GetStudentPaymentsAsync(studentId.Value);
        return Success(result, "Student payments retrieved successfully");
    }

    /// <summary>
    /// Creates a new student record. Restricted to Administrator.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentRequest request)
    {
        var result = await _studentService.CreateStudentAsync(request);
        return CreatedSuccess(nameof(GetStudentById), new { id = result.StudentId }, result, "Student registered successfully");
    }

    /// <summary>
    /// Updates student details. Admin can update any student; Students can only update their own profile.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentRequest request)
    {
        if (!IsAdmin())
        {
            var loggedInStudentId = GetAuthenticatedStudentId();
            if (!loggedInStudentId.HasValue || loggedInStudentId.Value != id)
            {
                throw new ForbiddenException("Access denied. You can only update your own student profile.");
            }
        }

        var result = await _studentService.UpdateStudentAsync(id, request);
        return Success(result, "Student profile updated successfully");
    }

    /// <summary>
    /// Soft deletes a student record. Restricted to Administrator.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        await _studentService.DeleteStudentAsync(id);
        return Success<object?>(null, "Student soft-deleted successfully");
    }
}
