using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Enrollments;
using TrainingCenter.Api.DTOs.Payments;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Controllers;

[Tags("Enrollments")]
public class EnrollmentsController : BaseApiController
{
    private readonly IEnrollmentService _enrollmentService;
    private readonly IPaymentService _paymentService;

    public EnrollmentsController(IEnrollmentService enrollmentService, IPaymentService paymentService)
    {
        _enrollmentService = enrollmentService;
        _paymentService = paymentService;
    }

    /// <summary>
    /// Return enrollments with filters: status, trackId, studentId, paymentStatus.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EnrollmentListItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEnrollments([FromQuery] EnrollmentFilterParams filters)
    {
        var enrollments = await _enrollmentService.GetEnrollmentsAsync(filters);
        return Ok(ApiResponse<PagedResult<EnrollmentListItemResponse>>.SuccessResponse(enrollments, "Enrollments retrieved successfully."));
    }

    /// <summary>
    /// Return enrollment details with student, track and payments.
    /// </summary>
    [HttpGet("{id:int}", Name = "GetEnrollmentById")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEnrollmentById([FromRoute] int id)
    {
        var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);
        return Ok(ApiResponse<EnrollmentDetailsResponse>.SuccessResponse(enrollment, "Enrollment details retrieved successfully."));
    }

    /// <summary>
    /// Enroll student in track using duplicate and capacity rules.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDetailsResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateEnrollment([FromBody] CreateEnrollmentRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse("Validation failed.", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(), 400));
        }

        var created = await _enrollmentService.CreateEnrollmentAsync(request);
        return CreatedAtRoute("GetEnrollmentById", new { id = created.EnrollmentId }, ApiResponse<EnrollmentDetailsResponse>.SuccessResponse(created, "Student enrolled in training track successfully.", 201));
    }

    /// <summary>
    /// Change enrollment status using valid transitions.
    /// </summary>
    [HttpPut("{id:int}/status")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEnrollmentStatus([FromRoute] int id, [FromBody] UpdateEnrollmentStatusRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse("Validation failed.", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(), 400));
        }

        var updated = await _enrollmentService.UpdateEnrollmentStatusAsync(id, request);
        return Ok(ApiResponse<EnrollmentDetailsResponse>.SuccessResponse(updated, "Enrollment status updated successfully."));
    }

    /// <summary>
    /// Return payment history for an enrollment.
    /// </summary>
    [HttpGet("{id:int}/payments")]
    [ProducesResponseType(typeof(ApiResponse<List<PaymentResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEnrollmentPayments([FromRoute] int id)
    {
        var payments = await _paymentService.GetPaymentsByEnrollmentIdAsync(id);
        return Ok(ApiResponse<List<PaymentResponse>>.SuccessResponse(payments, "Enrollment payment history retrieved successfully."));
    }
}
