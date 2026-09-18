using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Reports;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Controllers;

[Tags("Reports & Analytics")]
public class ReportsController : BaseApiController
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Query 14: Revenue Summary.
    /// Returns total revenue, realized revenue, expected revenue, paid count, pending count, and failed count.
    /// Uses decimal money representation to avoid financial rounding errors.
    /// </summary>
    /// <response code="200">Revenue summary metrics retrieved successfully.</response>
    [HttpGet("revenue-summary")]
    [ProducesResponseType(typeof(ApiResponse<RevenueSummaryReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRevenueSummary()
    {
        var summary = await _reportService.GetRevenueSummaryAsync();
        return Ok(ApiResponse<RevenueSummaryReportDto>.SuccessResponse(summary, "Revenue summary retrieved successfully."));
    }

    /// <summary>
    /// Query 15: Revenue Per Track.
    /// Groups paid payments by track and returns track title, total paid, enrollment count, and expected revenue.
    /// </summary>
    /// <response code="200">Revenue per track breakdown retrieved successfully.</response>
    [HttpGet("revenue-by-track")]
    [ProducesResponseType(typeof(ApiResponse<List<RevenueByTrackReportDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRevenueByTrack()
    {
        var report = await _reportService.GetRevenueByTrackReportAsync();
        return Ok(ApiResponse<List<RevenueByTrackReportDto>>.SuccessResponse(report, "Revenue by track report retrieved successfully."));
    }

    /// <summary>
    /// Query 16: Top Tracks By Enrollment.
    /// Returns tracks ordered by active enrollment count in descending order. Returns top 5 by default.
    /// </summary>
    /// <param name="count">Number of top tracks to return (default: 5).</param>
    /// <response code="200">Top tracks list retrieved successfully.</response>
    [HttpGet("top-tracks")]
    [ProducesResponseType(typeof(ApiResponse<List<TopTrackReportDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopTracks([FromQuery] int count = 5)
    {
        if (count < 1) count = 5;
        var report = await _reportService.GetTopTracksByEnrollmentAsync(count);
        return Ok(ApiResponse<List<TopTrackReportDto>>.SuccessResponse(report, $"Top {report.Count} tracks by active enrollment retrieved successfully."));
    }

    /// <summary>
    /// Query 17: Instructor Workload.
    /// Returns each instructor with number of tracks, active tracks, and active students supervised for management reporting.
    /// </summary>
    /// <response code="200">Instructor workload metrics retrieved successfully.</response>
    [HttpGet("instructor-workload")]
    [ProducesResponseType(typeof(ApiResponse<List<InstructorWorkloadReportDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInstructorWorkload()
    {
        var report = await _reportService.GetInstructorWorkloadReportAsync();
        return Ok(ApiResponse<List<InstructorWorkloadReportDto>>.SuccessResponse(report, "Instructor workload metrics retrieved successfully."));
    }

    /// <summary>
    /// Query 18: Students Without Payments.
    /// Returns students with active or pending enrollments who have no completed payments.
    /// Strictly excludes cancelled enrollments.
    /// </summary>
    /// <response code="200">Students without payments retrieved successfully.</response>
    [HttpGet("students-without-payments")]
    [ProducesResponseType(typeof(ApiResponse<List<StudentWithoutPaymentReportDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentsWithoutPayments()
    {
        var report = await _reportService.GetStudentsWithoutPaymentsAsync();
        return Ok(ApiResponse<List<StudentWithoutPaymentReportDto>>.SuccessResponse(report, "Students with active/pending enrollments and zero payments retrieved successfully."));
    }

    /// <summary>
    /// Query 20: Dashboard Summary.
    /// Returns high-level system numbers in one unified response (studentsCount, tracksCount, activeEnrollments, revenue, unpaidCount).
    /// </summary>
    /// <response code="200">Dashboard summary metrics retrieved successfully.</response>
    [HttpGet("dashboard-summary")]
    [ProducesResponseType(typeof(ApiResponse<DashboardSummaryReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardSummary()
    {
        var summary = await _reportService.GetDashboardSummaryAsync();
        return Ok(ApiResponse<DashboardSummaryReportDto>.SuccessResponse(summary, "Dashboard summary metrics retrieved successfully."));
    }

    /// <summary>
    /// Supporting Report: Unpaid and Partially Paid Enrollments.
    /// </summary>
    /// <response code="200">Unpaid enrollments report retrieved successfully.</response>
    [HttpGet("unpaid-enrollments")]
    [ProducesResponseType(typeof(ApiResponse<List<UnpaidEnrollmentReportDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnpaidEnrollments()
    {
        var report = await _reportService.GetUnpaidEnrollmentsAsync();
        return Ok(ApiResponse<List<UnpaidEnrollmentReportDto>>.SuccessResponse(report, "Unpaid and partially paid enrollments retrieved successfully."));
    }

    /// <summary>
    /// Supporting Report: Track Capacity and Available Seats.
    /// </summary>
    /// <response code="200">Track capacity report retrieved successfully.</response>
    [HttpGet("track-capacity")]
    [ProducesResponseType(typeof(ApiResponse<List<TrackCapacityReportDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrackCapacityReport()
    {
        var report = await _reportService.GetTrackCapacityReportAsync();
        return Ok(ApiResponse<List<TrackCapacityReportDto>>.SuccessResponse(report, "Track capacity report retrieved successfully."));
    }
}
