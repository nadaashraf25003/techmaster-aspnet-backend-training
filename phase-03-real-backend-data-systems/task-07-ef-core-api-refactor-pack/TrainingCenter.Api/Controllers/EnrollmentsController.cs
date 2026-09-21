using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Enrollments;
using TrainingCenter.Api.DTOs.Payments;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Controllers;

/// <summary>
/// REFACTORED PRODUCTION-READY ENROLLMENTS CONTROLLER.
/// Replaces BadEnrollmentsController by introducing strict DTO contracts,
/// asynchronous execution, service layer separation, domain validation,
/// server-side pagination, projection, soft-deletion, and standard HTTP status codes.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    /// <summary>
    /// Retrieves paginated and filtered enrollments with projection to avoid circular references and entity leaks.
    /// </summary>
    /// <param name="parameters">Query and pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EnrollmentResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<EnrollmentResponseDto>>>> GetAll(
        [FromQuery] EnrollmentQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var result = await _enrollmentService.GetAllPagedAsync(parameters, cancellationToken);
        return Ok(ApiResponse<PagedResult<EnrollmentResponseDto>>.SuccessResponse(result, "Enrollments retrieved successfully"));
    }

    /// <summary>
    /// Retrieves full enrollment details by ID including payment history.
    /// </summary>
    /// <param name="id">Enrollment ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDetailResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EnrollmentDetailResponseDto>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _enrollmentService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<EnrollmentDetailResponseDto>.SuccessResponse(result, "Enrollment details retrieved successfully"));
    }

    /// <summary>
    /// Creates a new student enrollment enforcing duplicate check, track capacity guard, and track active status.
    /// </summary>
    /// <param name="request">Creation DTO</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<EnrollmentResponseDto>>> Create(
        [FromBody] CreateEnrollmentRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _enrollmentService.CreateAsync(request, cancellationToken);
        var response = ApiResponse<EnrollmentResponseDto>.SuccessResponse(result, "Student enrolled successfully", StatusCodes.Status201Created);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, response);
    }

    /// <summary>
    /// Processes an installment or full payment against an enrollment with balance check and positive amount validation.
    /// </summary>
    /// <param name="request">Payment processing DTO</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost("pay")]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> Pay(
        [FromBody] ProcessPaymentRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _enrollmentService.ProcessPaymentAsync(request, cancellationToken);
        return Ok(ApiResponse<PaymentResponseDto>.SuccessResponse(result, "Payment processed successfully"));
    }

    /// <summary>
    /// Performs a soft delete on an enrollment, marking IsDeleted=true and preserving financial and audit trails.
    /// </summary>
    /// <param name="id">Enrollment ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        await _enrollmentService.SoftDeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
