using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Payments;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Controllers;

/// <summary>
/// Handles student payments with strict overpayment prevention, positive amount checks, and enrollment status updates.
/// </summary>
public class PaymentsController : BaseApiController
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// Retrieves a paginated and filtered list of payment transactions.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PaymentResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPayments([FromQuery] PaymentFilterParams filters)
    {
        var result = await _paymentService.GetPaymentsAsync(filters);
        return Success(result, "Payments retrieved successfully");
    }

    /// <summary>
    /// Retrieves payment details by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentById(int id)
    {
        var result = await _paymentService.GetPaymentByIdAsync(id);
        return Success(result, "Payment details retrieved successfully");
    }

    /// <summary>
    /// Records a new payment. Validates positive amount, non-cancelled enrollment, and rejects overpayment.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
    {
        var result = await _paymentService.CreatePaymentAsync(request);
        return CreatedSuccess(nameof(GetPaymentById), new { id = result.PaymentId }, result, "Payment recorded successfully");
    }

    /// <summary>
    /// Updates the status of a payment transaction (e.g. mark as Completed, Failed, or Refunded).
    /// </summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] UpdatePaymentStatusRequest request)
    {
        var result = await _paymentService.UpdatePaymentStatusAsync(id, request);
        return Success(result, "Payment status updated successfully");
    }
}
