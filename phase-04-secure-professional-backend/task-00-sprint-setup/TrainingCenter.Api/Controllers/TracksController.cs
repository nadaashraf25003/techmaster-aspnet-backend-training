using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Tracks;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Controllers;

/// <summary>
/// Manages training tracks with capacity, date validation, and instructor assignment rules.
/// </summary>
public class TracksController : BaseApiController
{
    private readonly ITrackService _trackService;

    public TracksController(ITrackService trackService)
    {
        _trackService = trackService;
    }

    /// <summary>
    /// Retrieves a paginated and filtered list of training tracks.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<TrackResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTracks([FromQuery] TrackFilterParams filters)
    {
        var result = await _trackService.GetTracksAsync(filters);
        return Success(result, "Training tracks retrieved successfully");
    }

    /// <summary>
    /// Retrieves details of a specific training track by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<TrackResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTrackById(int id)
    {
        var result = await _trackService.GetTrackByIdAsync(id);
        return Success(result, "Training track details retrieved successfully");
    }

    /// <summary>
    /// Creates a new training track with business rules validation (Capacity &gt; 0, StartDate &lt; EndDate, active instructor).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TrackResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateTrack([FromBody] CreateTrackRequest request)
    {
        var result = await _trackService.CreateTrackAsync(request);
        return CreatedSuccess(nameof(GetTrackById), new { id = result.TrainingTrackId }, result, "Training track created successfully");
    }

    /// <summary>
    /// Updates an existing training track. Ensures capacity cannot be reduced below current active enrollment count.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<TrackResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateTrack(int id, [FromBody] UpdateTrackRequest request)
    {
        var result = await _trackService.UpdateTrackAsync(id, request);
        return Success(result, "Training track updated successfully");
    }

    /// <summary>
    /// Deletes a track if it has no active or paid enrollments.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTrack(int id)
    {
        await _trackService.DeleteTrackAsync(id);
        return Success<object?>(null, "Training track deleted successfully");
    }
}
