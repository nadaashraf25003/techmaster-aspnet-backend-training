using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Enrollments;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Controllers;

[Tags("Enrollments")]
public class EnrollmentsController : BaseApiController
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    /// <summary>
    /// Query 19: Advanced Enrollment Filter.
    /// Combines multiple filters safely (trackId, status, paymentStatus, studentId, date range, search) using conditional IQueryable composition.
    /// Only applies filter predicates when parameters have value.
    /// </summary>
    /// <param name="filters">Query parameters: trackId, status, paymentStatus, studentId, from, to, search, pageNumber, pageSize.</param>
    /// <response code="200">Filtered enrollments retrieved successfully.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EnrollmentListItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEnrollments([FromQuery] EnrollmentFilterParams filters)
    {
        var enrollments = await _enrollmentService.GetEnrollmentsAsync(filters);
        return Ok(ApiResponse<PagedResult<EnrollmentListItemResponse>>.SuccessResponse(enrollments, "Enrollments retrieved successfully."));
    }

    /// <summary>
    /// Return enrollment details with student, track, and payment breakdown.
    /// </summary>
    [HttpGet("{id:int}", Name = "GetEnrollmentById")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEnrollmentById([FromRoute] int id)
    {
        var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);
        return Ok(ApiResponse<EnrollmentDetailsResponse>.SuccessResponse(enrollment, "Enrollment details retrieved successfully."));
    }

    /// <summary>
    /// Create new enrollment with student existence, duplicate check, and capacity enforcement.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDetailsResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateEnrollment([FromBody] CreateEnrollmentRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse("Validation failed.", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(), 400));
        }

        var created = await _enrollmentService.CreateEnrollmentAsync(request);
        return CreatedAtRoute("GetEnrollmentById", new { id = created.EnrollmentId }, ApiResponse<EnrollmentDetailsResponse>.SuccessResponse(created, "Student enrolled successfully.", 201));
    }

    /// <summary>
    /// Update enrollment status, progress percentage, or final grade.
    /// </summary>
    [HttpPut("{id:int}/status")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEnrollmentStatus([FromRoute] int id, [FromBody] UpdateEnrollmentStatusRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse("Validation failed.", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(), 400));
        }

        var updated = await _enrollmentService.UpdateEnrollmentStatusAsync(id, request);
        return Ok(ApiResponse<EnrollmentDetailsResponse>.SuccessResponse(updated, "Enrollment status updated successfully."));
    }

    /// <summary>
    /// Soft delete enrollment by id.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEnrollment([FromRoute] int id)
    {
        await _enrollmentService.DeleteEnrollmentAsync(id);
        return Ok(ApiResponse<object>.SuccessResponse(new { id }, "Enrollment deleted successfully."));
    }
}
