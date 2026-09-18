using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Reports;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Controllers;

[Tags("Reports")]
public class ReportsController : BaseApiController
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Return high-level dashboard numbers and operational KPIs.
    /// </summary>
    [HttpGet("dashboard-summary")]
    [ProducesResponseType(typeof(ApiResponse<DashboardSummaryReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardSummary()
    {
        var summary = await _reportService.GetDashboardSummaryAsync();
        return Ok(ApiResponse<DashboardSummaryReportDto>.SuccessResponse(summary, "Dashboard summary metrics retrieved successfully."));
    }

    /// <summary>
    /// Return unpaid or partially paid enrollments with outstanding amounts.
    /// </summary>
    [HttpGet("unpaid-enrollments")]
    [ProducesResponseType(typeof(ApiResponse<List<UnpaidEnrollmentReportDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnpaidEnrollments()
    {
        var report = await _reportService.GetUnpaidEnrollmentsAsync();
        return Ok(ApiResponse<List<UnpaidEnrollmentReportDto>>.SuccessResponse(report, "Unpaid and partially paid enrollments retrieved successfully."));
    }

    /// <summary>
    /// Return capacity, active enrollment count, and available seats per track.
    /// </summary>
    [HttpGet("track-capacity")]
    [ProducesResponseType(typeof(ApiResponse<List<TrackCapacityReportDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrackCapacityReport()
    {
        var report = await _reportService.GetTrackCapacityReportAsync();
        return Ok(ApiResponse<List<TrackCapacityReportDto>>.SuccessResponse(report, "Track capacity report retrieved successfully."));
    }

    /// <summary>
    /// Return revenue totals, realized revenue, and payment transactions count.
    /// </summary>
    [HttpGet("revenue-summary")]
    [ProducesResponseType(typeof(ApiResponse<RevenueSummaryReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRevenueSummary()
    {
        var report = await _reportService.GetRevenueSummaryAsync();
        return Ok(ApiResponse<RevenueSummaryReportDto>.SuccessResponse(report, "Revenue summary retrieved successfully."));
    }

    /// <summary>
    /// Return revenue grouped by training track with expected vs realized amounts.
    /// </summary>
    [HttpGet("revenue-by-track")]
    [ProducesResponseType(typeof(ApiResponse<List<RevenueByTrackReportDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRevenueByTrack()
    {
        var report = await _reportService.GetRevenueByTrackReportAsync();
        return Ok(ApiResponse<List<RevenueByTrackReportDto>>.SuccessResponse(report, "Revenue by track report retrieved successfully."));
    }

    /// <summary>
    /// Return instructor workload report with active tracks and supervised students.
    /// </summary>
    [HttpGet("instructor-workload")]
    [ProducesResponseType(typeof(ApiResponse<List<InstructorWorkloadReportDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInstructorWorkload()
    {
        var report = await _reportService.GetInstructorWorkloadReportAsync();
        return Ok(ApiResponse<List<InstructorWorkloadReportDto>>.SuccessResponse(report, "Instructor workload metrics retrieved successfully."));
    }
}
