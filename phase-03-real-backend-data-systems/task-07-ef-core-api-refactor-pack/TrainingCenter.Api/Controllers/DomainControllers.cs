using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Instructors;
using TrainingCenter.Api.DTOs.Payments;
using TrainingCenter.Api.DTOs.Reports;
using TrainingCenter.Api.DTOs.Students;
using TrainingCenter.Api.DTOs.Tracks;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> ProcessPayment(
        [FromBody] ProcessPaymentRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.ProcessPaymentAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<PaymentResponseDto>.SuccessResponse(result, "Payment processed successfully", StatusCodes.Status201Created));
    }

    [HttpGet("enrollment/{enrollmentId:int}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PaymentResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PaymentResponseDto>>>> GetByEnrollment(
        int enrollmentId,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.GetPaymentsByEnrollmentIdAsync(enrollmentId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<PaymentResponseDto>>.SuccessResponse(result, "Payments retrieved successfully"));
    }
}

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<StudentResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<StudentResponseDto>>>> GetAll(
        [FromQuery] PaginationParams pagination,
        CancellationToken cancellationToken)
    {
        var result = await _studentService.GetAllPagedAsync(pagination, cancellationToken);
        return Ok(ApiResponse<PagedResult<StudentResponseDto>>.SuccessResponse(result, "Students retrieved successfully"));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<StudentResponseDto>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _studentService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<StudentResponseDto>.SuccessResponse(result, "Student retrieved successfully"));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<StudentResponseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<StudentResponseDto>>> Create(
        [FromBody] CreateStudentRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _studentService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<StudentResponseDto>.SuccessResponse(result, "Student registered successfully", StatusCodes.Status201Created));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<StudentResponseDto>>> Update(
        int id,
        [FromBody] UpdateStudentRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _studentService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<StudentResponseDto>.SuccessResponse(result, "Student updated successfully"));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        await _studentService.SoftDeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TracksController : ControllerBase
{
    private readonly ITrackService _trackService;

    public TracksController(ITrackService trackService)
    {
        _trackService = trackService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TrackResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TrackResponseDto>>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _trackService.GetAllActiveAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<TrackResponseDto>>.SuccessResponse(result, "Tracks retrieved successfully"));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<TrackResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<TrackResponseDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _trackService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<TrackResponseDto>.SuccessResponse(result, "Track retrieved successfully"));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TrackResponseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<TrackResponseDto>>> Create(
        [FromBody] CreateTrackRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _trackService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<TrackResponseDto>.SuccessResponse(result, "Track created successfully", StatusCodes.Status201Created));
    }
}

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class InstructorsController : ControllerBase
{
    private readonly IInstructorService _instructorService;

    public InstructorsController(IInstructorService instructorService)
    {
        _instructorService = instructorService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<InstructorResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<InstructorResponseDto>>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _instructorService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<InstructorResponseDto>>.SuccessResponse(result, "Instructors retrieved successfully"));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<InstructorResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<InstructorResponseDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _instructorService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<InstructorResponseDto>.SuccessResponse(result, "Instructor retrieved successfully"));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<InstructorResponseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<InstructorResponseDto>>> Create(
        [FromBody] CreateInstructorRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _instructorService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<InstructorResponseDto>.SuccessResponse(result, "Instructor created successfully", StatusCodes.Status201Created));
    }
}

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("dashboard-summary")]
    [ProducesResponseType(typeof(ApiResponse<DashboardSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<DashboardSummaryDto>>> GetDashboardSummary(CancellationToken cancellationToken)
    {
        var result = await _reportService.GetDashboardSummaryAsync(cancellationToken);
        return Ok(ApiResponse<DashboardSummaryDto>.SuccessResponse(result, "Dashboard summary calculated successfully"));
    }

    [HttpGet("occupancy")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TrackOccupancyDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TrackOccupancyDto>>>> GetTrackOccupancy(CancellationToken cancellationToken)
    {
        var result = await _reportService.GetTrackOccupancyAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<TrackOccupancyDto>>.SuccessResponse(result, "Track occupancy retrieved successfully"));
    }
}
