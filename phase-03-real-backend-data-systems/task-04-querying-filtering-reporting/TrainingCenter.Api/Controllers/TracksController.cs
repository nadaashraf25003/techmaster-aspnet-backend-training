using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Tracks;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Controllers;

[Tags("Training Tracks")]
public class TracksController : BaseApiController
{
    private readonly ITrackService _trackService;

    public TracksController(ITrackService trackService)
    {
        _trackService = trackService;
    }

    /// <summary>
    /// Return paginated list of training tracks with search, status, and price filters.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<TrackResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTracks([FromQuery] TrackFilterParams filters)
    {
        var tracks = await _trackService.GetTracksAsync(filters);
        return Ok(ApiResponse<PagedResult<TrackResponse>>.SuccessResponse(tracks, "Training tracks retrieved successfully."));
    }

    /// <summary>
    /// Return training track details by ID.
    /// </summary>
    [HttpGet("{id:int}", Name = "GetTrackById")]
    [ProducesResponseType(typeof(ApiResponse<TrackResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTrackById([FromRoute] int id)
    {
        var track = await _trackService.GetTrackByIdAsync(id);
        return Ok(ApiResponse<TrackResponse>.SuccessResponse(track, "Training track details retrieved successfully."));
    }

    /// <summary>
    /// Create a new training track.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TrackResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateTrack([FromBody] CreateTrackRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse("Validation failed.", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(), 400));
        }

        var created = await _trackService.CreateTrackAsync(request);
        return CreatedAtRoute("GetTrackById", new { id = created.TrainingTrackId }, ApiResponse<TrackResponse>.SuccessResponse(created, "Training track created successfully.", 201));
    }

    /// <summary>
    /// Update training track by ID.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<TrackResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTrack([FromRoute] int id, [FromBody] UpdateTrackRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse("Validation failed.", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(), 400));
        }

        var updated = await _trackService.UpdateTrackAsync(id, request);
        return Ok(ApiResponse<TrackResponse>.SuccessResponse(updated, "Training track updated successfully."));
    }

    /// <summary>
    /// Soft delete training track by ID.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTrack([FromRoute] int id)
    {
        await _trackService.DeleteTrackAsync(id);
        return Ok(ApiResponse<object>.SuccessResponse(new { id }, "Training track deleted successfully."));
    }
}
