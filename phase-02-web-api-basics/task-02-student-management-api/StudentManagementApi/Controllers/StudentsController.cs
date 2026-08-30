using Microsoft.AspNetCore.Mvc;
using StudentManagementApi.DTOs;
using StudentManagementApi.Services;

namespace StudentManagementApi.Controllers;

/// <summary>
/// API Controller for managing student profiles and operations at TechMaster Academy.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(IStudentService studentService, ILogger<StudentsController> logger)
    {
        _studentService = studentService;
        _logger = logger;
    }

    /// <summary>
    /// Feature 01: Create a new student profile.
    /// </summary>
    /// <param name="dto">Student creation payload.</param>
    /// <returns>The newly created student profile.</returns>
    /// <response code="201">Student successfully created.</response>
    /// <response code="400">Invalid payload or duplicate email address.</response>
    [HttpPost]
    [ProducesResponseType(typeof(StudentResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, errorMessage, student) = await _studentService.CreateStudentAsync(dto);
        if (!success)
        {
            return BadRequest(new ApiErrorResponse { Message = errorMessage ?? "Failed to create student." });
        }

        return CreatedAtAction(
            nameof(GetStudentById),
            new { id = student!.Id },
            student
        );
    }

    /// <summary>
    /// Feature 02: Get all students with optional search, filter, and pagination.
    /// </summary>
    /// <param name="query">Search, track filter, active status filter, and pagination parameters.</param>
    /// <returns>A paginated list of students.</returns>
    /// <response code="200">Returns the paginated student list.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<StudentResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllStudents([FromQuery] StudentQueryParametersDto query)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _studentService.GetAllStudentsAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// Feature 06: Get aggregate student statistics for management.
    /// </summary>
    /// <returns>Overall statistics including total, active, inactive, and count by track.</returns>
    /// <response code="200">Returns the aggregated student statistics.</response>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(StudentStatsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentStats()
    {
        var stats = await _studentService.GetStudentStatsAsync();
        return Ok(stats);
    }

    /// <summary>
    /// Feature 03: Get a single student by unique ID.
    /// </summary>
    /// <param name="id">The unique student ID.</param>
    /// <returns>The student profile details.</returns>
    /// <response code="200">Student found and returned.</response>
    /// <response code="404">Student with the specified ID was not found.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(StudentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentById([FromRoute] int id)
    {
        var student = await _studentService.GetStudentByIdAsync(id);
        if (student == null)
        {
            return NotFound(new ApiErrorResponse
            {
                Message = $"Student with ID {id} was not found."
            });
        }

        return Ok(student);
    }

    /// <summary>
    /// Feature 04: Update an existing student's details.
    /// </summary>
    /// <param name="id">The unique student ID to update.</param>
    /// <param name="dto">The updated student data.</param>
    /// <returns>The updated student profile.</returns>
    /// <response code="200">Student successfully updated.</response>
    /// <response code="400">Invalid payload or email conflict.</response>
    /// <response code="404">Student with the specified ID was not found.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(StudentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStudent([FromRoute] int id, [FromBody] UpdateStudentDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, errorMessage, student) = await _studentService.UpdateStudentAsync(id, dto);
        if (!success)
        {
            if (errorMessage != null && errorMessage.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new ApiErrorResponse { Message = errorMessage });
            }

            return BadRequest(new ApiErrorResponse { Message = errorMessage ?? "Failed to update student." });
        }

        return Ok(student);
    }

    /// <summary>
    /// Feature 05: Activate or deactivate a student without deleting history.
    /// </summary>
    /// <param name="id">The unique student ID.</param>
    /// <param name="dto">The new active status.</param>
    /// <returns>Status confirmation message and updated student record.</returns>
    /// <response code="200">Student status successfully updated.</response>
    /// <response code="404">Student with the specified ID was not found.</response>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(StudentStatusResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStudentStatus([FromRoute] int id, [FromBody] UpdateStudentStatusDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, errorMessage, student) = await _studentService.UpdateStudentStatusAsync(id, dto.IsActive);
        if (!success)
        {
            return NotFound(new ApiErrorResponse { Message = errorMessage ?? $"Student with ID {id} was not found." });
        }

        var statusText = dto.IsActive ? "Active" : "Inactive";
        return Ok(new StudentStatusResponseDto
        {
            Message = $"Student status successfully updated to {statusText}.",
            Student = student!
        });
    }
}
