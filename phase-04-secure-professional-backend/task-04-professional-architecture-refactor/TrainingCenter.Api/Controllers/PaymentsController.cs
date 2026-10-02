using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.Data;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Payments;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Controllers;

/// <summary>
/// Handles payment transactions, payment gateway integrations, and payment status updates.
/// </summary>
public class PaymentsController : BaseApiController
{
    private readonly IPaymentService _paymentService;
    private readonly TrainingCenterDbContext _context;

    public PaymentsController(IPaymentService paymentService, TrainingCenterDbContext context)
    {
        _paymentService = paymentService;
        _context = context;
    }

    /// <summary>
    /// Retrieves a paginated and filtered list of payment transactions. Restricted to Administrators.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PaymentResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPayments([FromQuery] PaymentFilterParams filters)
    {
        var result = await _paymentService.GetPaymentsAsync(filters);
        return Success(result, "Payments retrieved successfully");
    }

    /// <summary>
    /// Retrieves payment details by ID. Admins can view any payment; Students can only view their own payments.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentById(int id)
    {
        var result = await _paymentService.GetPaymentByIdAsync(id);

        if (IsStudent())
        {
            var studentId = GetAuthenticatedStudentId();
            var enrollment = await _context.Enrollments.AsNoTracking().FirstOrDefaultAsync(e => e.EnrollmentId == result.EnrollmentId);
            if (enrollment == null || enrollment.StudentId != studentId)
            {
                throw new ForbiddenException("Access denied. You can only view your own payment details.");
            }
        }

        return Success(result, "Payment details retrieved successfully");
    }

    /// <summary>
    /// Records a new payment against an enrollment. Students can pay for their own enrollments; Admins can record payments for anyone.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Student")]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
    {
        if (IsStudent())
        {
            var studentId = GetAuthenticatedStudentId();
            var enrollment = await _context.Enrollments.AsNoTracking().FirstOrDefaultAsync(e => e.EnrollmentId == request.EnrollmentId);
            if (enrollment == null || enrollment.StudentId != studentId)
            {
                throw new ForbiddenException("Access denied. You can only create payments for your own enrollments.");
            }
        }

        var result = await _paymentService.CreatePaymentAsync(request);
        return CreatedSuccess(nameof(GetPaymentById), new { id = result.PaymentId }, result, "Payment recorded successfully");
    }

    /// <summary>
    /// Updates the status of a payment transaction (e.g. mark as Completed, Failed, or Refunded).
    /// Admin Role Story: Only Administrators can update payment statuses. Students/Instructors receive 403 Forbidden.
    /// </summary>
    [HttpPut("{id:int}/status")]
    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] UpdatePaymentStatusRequest request)
    {
        var result = await _paymentService.UpdatePaymentStatusAsync(id, request);
        return Success(result, "Payment status updated successfully");
    }
}
