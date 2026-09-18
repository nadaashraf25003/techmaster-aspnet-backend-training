using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Payments;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Controllers;

[Tags("Payments")]
public class PaymentsController : BaseApiController
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// Query 13: Payments By Date Range.
    /// Returns payments inside date range (from/to), validating that from &lt;= to.
    /// Supports status and method filtering with server-side pagination.
    /// </summary>
    /// <param name="filters">Query parameters: from (startDate), to (endDate), status, method, enrollmentId, pageNumber, pageSize.</param>
    /// <response code="200">Payments retrieved successfully.</response>
    /// <response code="400">Validation error when 'from' date is after 'to' date.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PaymentResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPayments([FromQuery] PaymentFilterParams filters)
    {
        if (!filters.IsDateRangeValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Invalid date range: 'from' date must be less than or equal to 'to' date.",
                new List<string> { $"'from' ({filters.From:yyyy-MM-dd}) cannot be greater than 'to' ({filters.To:yyyy-MM-dd})." },
                StatusCodes.Status400BadRequest));
        }

        var payments = await _paymentService.GetPaymentsAsync(filters);
        return Ok(ApiResponse<PagedResult<PaymentResponse>>.SuccessResponse(payments, "Payments retrieved successfully."));
    }

    /// <summary>
    /// Return payment details by id.
    /// </summary>
    [HttpGet("{id:int}", Name = "GetPaymentById")]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentById([FromRoute] int id)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(id);
        return Ok(ApiResponse<PaymentResponse>.SuccessResponse(payment, "Payment details retrieved successfully."));
    }

    /// <summary>
    /// Create payment for an enrollment.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse("Validation failed.", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(), 400));
        }

        var created = await _paymentService.CreatePaymentAsync(request);
        return CreatedAtRoute("GetPaymentById", new { id = created.PaymentId }, ApiResponse<PaymentResponse>.SuccessResponse(created, "Payment processed successfully.", 201));
    }

    /// <summary>
    /// Update payment status.
    /// </summary>
    [HttpPut("{id:int}/status")]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePaymentStatus([FromRoute] int id, [FromBody] UpdatePaymentStatusRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse("Validation failed.", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(), 400));
        }

        var updated = await _paymentService.UpdatePaymentStatusAsync(id, request);
        return Ok(ApiResponse<PaymentResponse>.SuccessResponse(updated, "Payment status updated successfully."));
    }
}
