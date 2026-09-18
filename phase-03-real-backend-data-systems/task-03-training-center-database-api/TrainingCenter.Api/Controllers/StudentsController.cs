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
    /// Return paginated students with search and active status filters.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<StudentListItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudents([FromQuery] StudentFilterParams filters)
    {
        var result = await _studentService.GetStudentsAsync(filters);
        return Ok(ApiResponse<PagedResult<StudentListItemResponse>>.SuccessResponse(result, "Students retrieved successfully."));
    }

    /// <summary>
    /// Return one student by id with basic enrollment summary.
    /// </summary>
    [HttpGet("{id:int}", Name = "GetStudentById")]
    [ProducesResponseType(typeof(ApiResponse<StudentDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentById([FromRoute] int id)
    {
        var student = await _studentService.GetStudentByIdAsync(id);
        return Ok(ApiResponse<StudentDetailsResponse>.SuccessResponse(student, "Student details retrieved successfully."));
    }

    /// <summary>
    /// Create student with unique email validation.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<StudentDetailsResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse("Validation failed.", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(), 400));
        }

        var created = await _studentService.CreateStudentAsync(request);
        return CreatedAtRoute("GetStudentById", new { id = created.StudentId }, ApiResponse<StudentDetailsResponse>.SuccessResponse(created, "Student created successfully.", 201));
    }

    /// <summary>
    /// Update student information and UpdatedAt.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<StudentDetailsResponse>), StatusCodes.Status200OK)]
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
        return Ok(ApiResponse<StudentDetailsResponse>.SuccessResponse(updated, "Student updated successfully."));
    }

    /// <summary>
    /// Soft delete student, do not remove row permanently.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SoftDeleteStudent([FromRoute] int id)
    {
        await _studentService.SoftDeleteStudentAsync(id);
        return Ok(ApiResponse.SuccessMessage("Student was successfully soft deleted."));
    }

    /// <summary>
    /// Return student enrollment history.
    /// </summary>
    [HttpGet("{id:int}/enrollments")]
    [ProducesResponseType(typeof(ApiResponse<List<StudentEnrollmentSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentEnrollments([FromRoute] int id)
    {
        var enrollments = await _studentService.GetStudentEnrollmentsAsync(id);
        return Ok(ApiResponse<List<StudentEnrollmentSummaryDto>>.SuccessResponse(enrollments, "Student enrollment history retrieved successfully."));
    }
}
