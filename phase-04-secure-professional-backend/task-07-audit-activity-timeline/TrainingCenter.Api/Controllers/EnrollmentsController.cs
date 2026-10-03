using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Enrollments;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Controllers;

/// <summary>
/// Handles enrollment lifecycle and enforces capacity, active student, and duplicate enrollment restrictions.
/// </summary>
public class EnrollmentsController : BaseApiController
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    /// <summary>
    /// Retrieves a paginated and filtered list of all enrollments. Restricted to Administrators.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EnrollmentListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetEnrollments([FromQuery] EnrollmentFilterParams filters)
    {
        var result = await _enrollmentService.GetEnrollmentsAsync(filters);
        return Success(result, "Enrollments retrieved successfully");
    }

    /// <summary>
    /// Retrieves full details of an enrollment including student, track, and payment history.
    /// Admins can view any enrollment; Students can only view their own enrollment.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEnrollmentById(int id)
    {
        var result = await _enrollmentService.GetEnrollmentByIdAsync(id);

        if (!IsAdmin())
        {
            if (IsStudent())
            {
                var studentId = GetAuthenticatedStudentId();
                if (!studentId.HasValue || result.StudentId != studentId.Value)
                {
                    throw new ForbiddenException("Access denied. You can only view your own enrollments.");
                }
            }
        }

        return Success(result, "Enrollment details retrieved successfully");
    }

    /// <summary>
    /// Enrolls a student in a track. Students can enroll themselves; Admins can enroll any student.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Student")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDetailsResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateEnrollment([FromBody] CreateEnrollmentRequest request)
    {
        if (IsStudent())
        {
            var studentId = GetAuthenticatedStudentId();
            if (!studentId.HasValue)
            {
                throw new ForbiddenException("No student profile linked to the current user.");
            }
            request.StudentId = studentId.Value; // Prevent students from enrolling other students
        }

        var result = await _enrollmentService.CreateEnrollmentAsync(request);
        return CreatedSuccess(nameof(GetEnrollmentById), new { id = result.EnrollmentId }, result, "Enrollment registered successfully");
    }

    /// <summary>
    /// Updates the status of an enrollment (e.g. Activate, Complete, Cancel). Restricted to Administrators.
    /// </summary>
    [HttpPut("{id:int}/status")]
    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEnrollmentStatus(int id, [FromBody] UpdateEnrollmentStatusRequest request)
    {
        var result = await _enrollmentService.UpdateEnrollmentStatusAsync(id, request);
        return Success(result, "Enrollment status updated successfully");
    }

    /// <summary>
    /// Cancels or removes an enrollment. Restricted to Administrators.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEnrollment(int id)
    {
        await _enrollmentService.DeleteEnrollmentAsync(id);
        return Success<object?>(null, "Enrollment deleted successfully");
    }
}
