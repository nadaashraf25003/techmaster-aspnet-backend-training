using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Students;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Controllers;

[Tags("Students")]
public class StudentsController : BaseApiController
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    /// <summary>
    /// Return paginated list of students with optional search and active status filter.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<StudentResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudents([FromQuery] StudentFilterParams filters)
    {
        var students = await _studentService.GetStudentsAsync(filters);
        return Ok(ApiResponse<PagedResult<StudentResponse>>.SuccessResponse(students, "Students retrieved successfully."));
    }

    /// <summary>
    /// Return student profile by ID.
    /// </summary>
    [HttpGet("{id:int}", Name = "GetStudentById")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentById([FromRoute] int id)
    {
        var student = await _studentService.GetStudentByIdAsync(id);
        return Ok(ApiResponse<StudentResponse>.SuccessResponse(student, "Student profile retrieved successfully."));
    }

    /// <summary>
    /// Create a new student profile.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse("Validation failed.", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(), 400));
        }

        var created = await _studentService.CreateStudentAsync(request);
        return CreatedAtRoute("GetStudentById", new { id = created.StudentId }, ApiResponse<StudentResponse>.SuccessResponse(created, "Student created successfully.", 201));
    }

    /// <summary>
    /// Update student profile by ID.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateStudent([FromRoute] int id, [FromBody] UpdateStudentRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse("Validation failed.", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(), 400));
        }

        var updated = await _studentService.UpdateStudentAsync(id, request);
        return Ok(ApiResponse<StudentResponse>.SuccessResponse(updated, "Student updated successfully."));
    }

    /// <summary>
    /// Soft delete student by ID.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStudent([FromRoute] int id)
    {
        await _studentService.DeleteStudentAsync(id);
        return Ok(ApiResponse<object>.SuccessResponse(new { id }, "Student deleted successfully."));
    }
}
