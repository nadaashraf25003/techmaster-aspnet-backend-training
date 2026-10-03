using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Sessions;
using TrainingCenter.Api.DTOs.Students;
using TrainingCenter.Api.DTOs.Tracks;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Controllers;

/// <summary>
/// Manages course catalog, scheduling, capacity limits, and instructor assignments.
/// </summary>
public class TracksController : BaseApiController
{
    private readonly ITrackService _trackService;
    private readonly ITrackSessionService _sessionService;

    public TracksController(ITrackService trackService, ITrackSessionService sessionService)
    {
        _trackService = trackService;
        _sessionService = sessionService;
    }

    /// <summary>
    /// Retrieves a paginated and filtered list of training tracks. Publicly accessible.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<TrackResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTracks([FromQuery] TrackFilterParams filters)
    {
        var result = await _trackService.GetTracksAsync(filters);
        return Success(result, "Tracks retrieved successfully");
    }

    /// <summary>
    /// Student / Public Catalog Story: Browse active and upcoming tracks that have open seats available.
    /// Accessible via GET /api/tracks/available.
    /// </summary>
    [HttpGet("available")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<List<TrackResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableTracks()
    {
        var result = await _trackService.GetAvailableTracksAsync();
        return Success(result, "Available tracks retrieved successfully");
    }

    /// <summary>
    /// Retrieves single training track details by ID. Publicly accessible.
    /// </summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<TrackResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTrackById(int id)
    {
        var result = await _trackService.GetTrackByIdAsync(id);
        return Success(result, "Track details retrieved successfully");
    }

    /// <summary>
    /// Instructor/Admin Story: Retrieves students enrolled in a specific track.
    /// Admin has full access. Instructors can ONLY view students for their own assigned tracks (403 Forbidden otherwise).
    /// </summary>
    [HttpGet("{id:int}/students")]
    [HttpGet("{id:int}/enrollments")]
    [Authorize(Roles = "Admin,Instructor")]
    [ProducesResponseType(typeof(ApiResponse<List<StudentResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTrackStudents(int id)
    {
        var requestingInstructorId = GetAuthenticatedInstructorId();
        var isAdmin = IsAdmin();

        var result = await _trackService.GetEnrolledStudentsForTrackAsync(id, requestingInstructorId, isAdmin);
        return Success(result, "Enrolled students for track retrieved successfully");
    }

    /// <summary>
    /// Retrieves sessions associated with a specific training track.
    /// Restricted to Admin or the assigned Instructor.
    /// </summary>
    [HttpGet("{id:int}/sessions")]
    [Authorize(Roles = "Admin,Instructor")]
    [ProducesResponseType(typeof(ApiResponse<List<TrackSessionResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTrackSessions(int id)
    {
        var requestingInstructorId = GetAuthenticatedInstructorId();
        var isAdmin = IsAdmin();

        var result = await _sessionService.GetSessionsByTrackAsync(id, requestingInstructorId, isAdmin);
        return Success(result, "Track sessions retrieved successfully");
    }

    /// <summary>
    /// Admin Story: Assigns an instructor to a training track. Validates instructor existence and active status.
    /// Accessible via PUT /api/tracks/{id}/assign-instructor.
    /// </summary>
    [HttpPut("{id:int}/assign-instructor")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<TrackResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignInstructor(int id, [FromBody] AssignInstructorRequest request)
    {
        var result = await _trackService.AssignInstructorAsync(id, request.InstructorId);
        return Success(result, "Instructor assigned to track successfully");
    }

    /// <summary>
    /// Admin Story: Creates a new training track. Restricted to Administrators.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<TrackResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateTrack([FromBody] CreateTrackRequest request)
    {
        var result = await _trackService.CreateTrackAsync(request);
        return CreatedSuccess(nameof(GetTrackById), new { id = result.TrainingTrackId }, result, "Training track created successfully");
    }

    /// <summary>
    /// Admin Story: Updates training track details. Restricted to Administrators.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<TrackResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTrack(int id, [FromBody] UpdateTrackRequest request)
    {
        var result = await _trackService.UpdateTrackAsync(id, request);
        return Success(result, "Training track updated successfully");
    }

    /// <summary>
    /// Admin Story: Soft deletes a training track. Restricted to Administrators.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTrack(int id)
    {
        await _trackService.DeleteTrackAsync(id);
        return Success<object?>(null, "Training track soft-deleted successfully");
    }
}
