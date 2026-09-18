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
    /// Return paginated list of instructors with specialization and search filters.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<InstructorResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInstructors([FromQuery] InstructorFilterParams filters)
    {
        var instructors = await _instructorService.GetInstructorsAsync(filters);
        return Ok(ApiResponse<PagedResult<InstructorResponse>>.SuccessResponse(instructors, "Instructors retrieved successfully."));
    }

    /// <summary>
    /// Return instructor profile by ID.
    /// </summary>
    [HttpGet("{id:int}", Name = "GetInstructorById")]
    [ProducesResponseType(typeof(ApiResponse<InstructorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInstructorById([FromRoute] int id)
    {
        var instructor = await _instructorService.GetInstructorByIdAsync(id);
        return Ok(ApiResponse<InstructorResponse>.SuccessResponse(instructor, "Instructor profile retrieved successfully."));
    }

    /// <summary>
    /// Create a new instructor.
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
    /// Update instructor profile by ID.
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
    /// Soft delete instructor by ID.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteInstructor([FromRoute] int id)
    {
        await _instructorService.DeleteInstructorAsync(id);
        return Ok(ApiResponse<object>.SuccessResponse(new { id }, "Instructor deleted successfully."));
    }
}
