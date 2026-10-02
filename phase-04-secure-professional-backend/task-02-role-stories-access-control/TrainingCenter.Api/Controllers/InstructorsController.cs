using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Instructors;
using TrainingCenter.Api.DTOs.Students;
using TrainingCenter.Api.DTOs.Tracks;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Controllers;

/// <summary>
/// Manages instructor directory, specialized tracks, and instructor portal operations.
/// </summary>
public class InstructorsController : BaseApiController
{
    private readonly IInstructorService _instructorService;

    public InstructorsController(IInstructorService instructorService)
    {
        _instructorService = instructorService;
    }

    /// <summary>
    /// Retrieves a paginated list of instructors. Restricted to Administrators.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<InstructorResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetInstructors([FromQuery] InstructorFilterParams filters)
    {
        var result = await _instructorService.GetInstructorsAsync(filters);
        return Success(result, "Instructors retrieved successfully");
    }

    /// <summary>
    /// Retrieves instructor details by ID. Admins can view any instructor; Instructors can only view their own profile.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<InstructorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInstructorById(int id)
    {
        if (!IsAdmin())
        {
            var loggedInInstructorId = GetAuthenticatedInstructorId();
            if (!loggedInInstructorId.HasValue || loggedInInstructorId.Value != id)
            {
                throw new ForbiddenException("Access denied. You can only view your own instructor profile.");
            }
        }

        var result = await _instructorService.GetInstructorByIdAsync(id);
        return Success(result, "Instructor details retrieved successfully");
    }

    /// <summary>
    /// Instructor Portal Story: Retrieves tracks assigned to the currently authenticated instructor.
    /// Accessible via GET /api/instructors/me/tracks and GET /api/instructor/my-tracks.
    /// </summary>
    [HttpGet("me/tracks")]
    [HttpGet("/api/instructor/my-tracks")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(typeof(ApiResponse<List<TrackResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMyAssignedTracks()
    {
        var instructorId = GetAuthenticatedInstructorId();
        if (!instructorId.HasValue)
        {
            throw new ForbiddenException("No instructor profile linked to the current user.");
        }

        var result = await _instructorService.GetInstructorTracksAsync(instructorId.Value);
        return Success(result, "Assigned tracks retrieved successfully");
    }

    /// <summary>
    /// Instructor Portal Story: Retrieves all students enrolled across the instructor's assigned tracks.
    /// Accessible via GET /api/instructors/me/students and GET /api/instructor/my-students.
    /// </summary>
    [HttpGet("me/students")]
    [HttpGet("/api/instructor/my-students")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(typeof(ApiResponse<List<StudentResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMyStudents()
    {
        var instructorId = GetAuthenticatedInstructorId();
        if (!instructorId.HasValue)
        {
            throw new ForbiddenException("No instructor profile linked to the current user.");
        }

        var result = await _instructorService.GetInstructorStudentsAsync(instructorId.Value);
        return Success(result, "Assigned students retrieved successfully");
    }

    /// <summary>
    /// Creates a new instructor record. Restricted to Admin.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<InstructorResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateInstructor([FromBody] CreateInstructorRequest request)
    {
        var result = await _instructorService.CreateInstructorAsync(request);
        return CreatedSuccess(nameof(GetInstructorById), new { id = result.InstructorId }, result, "Instructor created successfully");
    }

    /// <summary>
    /// Updates instructor details. Restricted to Admin.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<InstructorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateInstructor(int id, [FromBody] UpdateInstructorRequest request)
    {
        var result = await _instructorService.UpdateInstructorAsync(id, request);
        return Success(result, "Instructor updated successfully");
    }

    /// <summary>
    /// Soft deletes an instructor record. Restricted to Admin.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteInstructor(int id)
    {
        await _instructorService.DeleteInstructorAsync(id);
        return Success<object?>(null, "Instructor soft-deleted successfully");
    }
}
