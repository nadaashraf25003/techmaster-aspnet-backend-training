using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Reports;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Controllers;

/// <summary>
/// Provides high-integrity business analytics and aggregate reports based on verified transactions.
/// </summary>
public class ReportsController : BaseApiController
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Returns overall revenue statistics, counting only completed transactions as realized revenue.
    /// </summary>
    [HttpGet("revenue-summary")]
    [ProducesResponseType(typeof(ApiResponse<RevenueSummaryReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRevenueSummary()
    {
        var result = await _reportService.GetRevenueSummaryAsync();
        return Success(result, "Revenue summary report retrieved successfully");
    }

    /// <summary>
    /// Returns revenue breakdown and collection rate by training track.
    /// </summary>
    [HttpGet("revenue-by-track")]
    [ProducesResponseType(typeof(ApiResponse<List<RevenueByTrackReportDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRevenueByTrack()
    {
        var result = await _reportService.GetRevenueByTrackReportAsync();
        return Success(result, "Revenue by track report retrieved successfully");
    }

    /// <summary>
    /// Returns top tracks ranked by active enrollment count and capacity utilization.
    /// </summary>
    [HttpGet("top-tracks")]
    [ProducesResponseType(typeof(ApiResponse<List<TopTrackReportDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopTracks([FromQuery] int count = 5)
    {
        var result = await _reportService.GetTopTracksByEnrollmentAsync(count);
        return Success(result, "Top tracks report retrieved successfully");
    }

    /// <summary>
    /// Returns workload statistics per instructor, including active tracks, supervised students, and generated revenue.
    /// </summary>
    [HttpGet("instructor-workload")]
    [ProducesResponseType(typeof(ApiResponse<List<InstructorWorkloadReportDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInstructorWorkload()
    {
        var result = await _reportService.GetInstructorWorkloadReportAsync();
        return Success(result, "Instructor workload report retrieved successfully");
    }

    /// <summary>
    /// Returns enrollments that have made zero completed payments and have an outstanding balance.
    /// </summary>
    [HttpGet("students-without-payments")]
    [ProducesResponseType(typeof(ApiResponse<List<StudentWithoutPaymentReportDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentsWithoutPayments()
    {
        var result = await _reportService.GetStudentsWithoutPaymentsAsync();
        return Success(result, "Students without payments report retrieved successfully");
    }

    /// <summary>
    /// Returns a comprehensive system dashboard overview.
    /// </summary>
    [HttpGet("dashboard-summary")]
    [ProducesResponseType(typeof(ApiResponse<DashboardSummaryReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardSummary()
    {
        var result = await _reportService.GetDashboardSummaryAsync();
        return Success(result, "Dashboard summary retrieved successfully");
    }

    /// <summary>
    /// Returns real-time capacity and occupancy metrics across all training tracks.
    /// </summary>
    [HttpGet("track-capacities")]
    [ProducesResponseType(typeof(ApiResponse<List<TrackCapacityReportDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrackCapacities()
    {
        var result = await _reportService.GetTrackCapacityReportAsync();
        return Success(result, "Track capacity report retrieved successfully");
    }
}
