using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Instructors;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Controllers;

[Tags("Instructors")]
public class InstructorsController : BaseApiController
{
    private readonly IInstructorService _instructorService;

    public InstructorsController(IInstructorService instructorService)
    {
        _instructorService = instructorService;
    }

    /// <summary>
    /// Return instructors list with optional active filter.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<InstructorResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInstructors([FromQuery] bool? isActive)
    {
        var instructors = await _instructorService.GetAllInstructorsAsync(isActive);
        return Ok(ApiResponse<List<InstructorResponse>>.SuccessResponse(instructors, "Instructors retrieved successfully."));
    }

    /// <summary>
    /// Return instructor details.
    /// </summary>
    [HttpGet("{id:int}", Name = "GetInstructorById")]
    [ProducesResponseType(typeof(ApiResponse<InstructorDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInstructorById([FromRoute] int id)
    {
        var instructor = await _instructorService.GetInstructorByIdAsync(id);
        return Ok(ApiResponse<InstructorDetailsResponse>.SuccessResponse(instructor, "Instructor details retrieved successfully."));
    }

    /// <summary>
    /// Create instructor with unique email validation.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<InstructorResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateInstructor([FromBody] CreateInstructorRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse("Validation failed.", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(), 400));
        }

        var created = await _instructorService.CreateInstructorAsync(request);
        return CreatedAtRoute("GetInstructorById", new { id = created.InstructorId }, ApiResponse<InstructorResponse>.SuccessResponse(created, "Instructor created successfully.", 201));
    }

    /// <summary>
    /// Update instructor information.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<InstructorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateInstructor([FromRoute] int id, [FromBody] UpdateInstructorRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse("Validation failed.", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(), 400));
        }

        var updated = await _instructorService.UpdateInstructorAsync(id, request);
        return Ok(ApiResponse<InstructorResponse>.SuccessResponse(updated, "Instructor updated successfully."));
    }

    /// <summary>
    /// Return tracks assigned to instructor.
    /// </summary>
    [HttpGet("{id:int}/tracks")]
    [ProducesResponseType(typeof(ApiResponse<List<InstructorTrackSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInstructorTracks([FromRoute] int id)
    {
        var tracks = await _instructorService.GetInstructorTracksAsync(id);
        return Ok(ApiResponse<List<InstructorTrackSummaryDto>>.SuccessResponse(tracks, "Instructor assigned tracks retrieved successfully."));
    }
}
