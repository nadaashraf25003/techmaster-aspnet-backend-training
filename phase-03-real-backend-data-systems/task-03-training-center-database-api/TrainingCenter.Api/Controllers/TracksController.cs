using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Tracks;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Controllers;

[Tags("Tracks")]
public class TracksController : BaseApiController
{
    private readonly ITrackService _trackService;

    public TracksController(ITrackService trackService)
    {
        _trackService = trackService;
    }

    /// <summary>
    /// Return tracks with filters: keyword, level, status, instructorId.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<TrackListItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTracks([FromQuery] TrackFilterParams filters)
    {
        var tracks = await _trackService.GetTracksAsync(filters);
        return Ok(ApiResponse<PagedResult<TrackListItemResponse>>.SuccessResponse(tracks, "Tracks retrieved successfully."));
    }

    /// <summary>
    /// Return track details with instructor and capacity summary.
    /// </summary>
    [HttpGet("{id:int}", Name = "GetTrackById")]
    [ProducesResponseType(typeof(ApiResponse<TrackDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTrackById([FromRoute] int id)
    {
        var track = await _trackService.GetTrackByIdAsync(id);
        return Ok(ApiResponse<TrackDetailsResponse>.SuccessResponse(track, "Track details retrieved successfully."));
    }

    /// <summary>
    /// Create track with instructor and capacity validation.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TrackDetailsResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateTrack([FromBody] CreateTrackRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse("Validation failed.", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(), 400));
        }

        var created = await _trackService.CreateTrackAsync(request);
        return CreatedAtRoute("GetTrackById", new { id = created.TrainingTrackId }, ApiResponse<TrackDetailsResponse>.SuccessResponse(created, "Training track created successfully.", 201));
    }

    /// <summary>
    /// Update track information.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<TrackDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateTrack([FromRoute] int id, [FromBody] UpdateTrackRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse("Validation failed.", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(), 400));
        }

        var updated = await _trackService.UpdateTrackAsync(id, request);
        return Ok(ApiResponse<TrackDetailsResponse>.SuccessResponse(updated, "Training track updated successfully."));
    }

    /// <summary>
    /// Soft delete track if no active enrollments exist.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SoftDeleteTrack([FromRoute] int id)
    {
        await _trackService.SoftDeleteTrackAsync(id);
        return Ok(ApiResponse.SuccessMessage("Training track was successfully soft deleted."));
    }

    /// <summary>
    /// Return students enrolled in a track.
    /// </summary>
    [HttpGet("{id:int}/students")]
    [ProducesResponseType(typeof(ApiResponse<List<TrackStudentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTrackStudents([FromRoute] int id)
    {
        var students = await _trackService.GetTrackStudentsAsync(id);
        return Ok(ApiResponse<List<TrackStudentDto>>.SuccessResponse(students, "Enrolled students retrieved successfully."));
    }
}
