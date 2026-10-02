using Microsoft.AspNetCore.Mvc;
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
    /// Retrieves a paginated and filtered list of enrollments.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EnrollmentListItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEnrollments([FromQuery] EnrollmentFilterParams filters)
    {
        var result = await _enrollmentService.GetEnrollmentsAsync(filters);
        return Success(result, "Enrollments retrieved successfully");
    }

    /// <summary>
    /// Retrieves full details of an enrollment including student, track, and payment history.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEnrollmentById(int id)
    {
        var result = await _enrollmentService.GetEnrollmentByIdAsync(id);
        return Success(result, "Enrollment details retrieved successfully");
    }

    /// <summary>
    /// Enrolls a student in a track. Enforces capacity limit, active student status, open track status, and duplicate prevention.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDetailsResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateEnrollment([FromBody] CreateEnrollmentRequest request)
    {
        var result = await _enrollmentService.CreateEnrollmentAsync(request);
        return CreatedSuccess(nameof(GetEnrollmentById), new { id = result.EnrollmentId }, result, "Enrollment registered successfully");
    }

    /// <summary>
    /// Updates the status of an enrollment (e.g. Activate, Complete, Cancel). Completed enrollments cannot be cancelled.
    /// </summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEnrollmentStatus(int id, [FromBody] UpdateEnrollmentStatusRequest request)
    {
        var result = await _enrollmentService.UpdateEnrollmentStatusAsync(id, request);
        return Success(result, "Enrollment status updated successfully");
    }

    /// <summary>
    /// Cancels or removes an enrollment. Blocks deletion if completed payments exist without formal cancellation.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEnrollment(int id)
    {
        await _enrollmentService.DeleteEnrollmentAsync(id);
        return Success<object?>(null, "Enrollment deleted successfully");
    }
}
