using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Instructors;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Controllers;

/// <summary>
/// Manages instructor profiles and prevents deletion of instructors assigned to active tracks.
/// </summary>
public class InstructorsController : BaseApiController
{
    private readonly IInstructorService _instructorService;

    public InstructorsController(IInstructorService instructorService)
    {
        _instructorService = instructorService;
    }

    /// <summary>
    /// Retrieves a paginated and filtered list of instructors.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<InstructorResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInstructors([FromQuery] InstructorFilterParams filters)
    {
        var result = await _instructorService.GetInstructorsAsync(filters);
        return Success(result, "Instructors retrieved successfully");
    }

    /// <summary>
    /// Retrieves instructor details by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<InstructorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInstructorById(int id)
    {
        var result = await _instructorService.GetInstructorByIdAsync(id);
        return Success(result, "Instructor details retrieved successfully");
    }

    /// <summary>
    /// Creates a new instructor profile.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<InstructorResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateInstructor([FromBody] CreateInstructorRequest request)
    {
        var result = await _instructorService.CreateInstructorAsync(request);
        return CreatedSuccess(nameof(GetInstructorById), new { id = result.InstructorId }, result, "Instructor created successfully");
    }

    /// <summary>
    /// Updates an existing instructor profile.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<InstructorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateInstructor(int id, [FromBody] UpdateInstructorRequest request)
    {
        var result = await _instructorService.UpdateInstructorAsync(id, request);
        return Success(result, "Instructor profile updated successfully");
    }

    /// <summary>
    /// Deletes an instructor if not currently assigned to active tracks.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteInstructor(int id)
    {
        await _instructorService.DeleteInstructorAsync(id);
        return Success<object?>(null, "Instructor deleted successfully");
    }
}
