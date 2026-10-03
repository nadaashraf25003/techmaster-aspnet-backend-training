using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.DTOs.ActivityLogs;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Controllers;

/// <summary>
/// Provides administrative audit trail, security events, and entity lifecycle timeline queries.
/// </summary>
[Authorize(Roles = "Admin")]
[Route("api/admin/activity-logs")]
public class ActivityLogsController : BaseApiController
{
    private readonly IAuditService _auditService;

    public ActivityLogsController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    /// <summary>
    /// Retrieves paginated and filtered activity logs. Restricted to Administrators.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ActivityLogListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetActivityLogs([FromQuery] ActivityLogFilterParams filters)
    {
        var result = await _auditService.GetActivityLogsAsync(filters);
        return Success(result, "Activity logs retrieved successfully");
    }

    /// <summary>
    /// Retrieves full details of a specific activity log entry. Restricted to Administrators.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ActivityLogListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetActivityLogById(int id)
    {
        var result = await _auditService.GetActivityLogByIdAsync(id);
        if (result == null)
        {
            throw new NotFoundException($"Activity log with ID {id} was not found.");
        }

        return Success(result, "Activity log details retrieved successfully");
    }

    /// <summary>
    /// Retrieves aggregated audit statistics, action breakdown, and recent activity feed. Restricted to Administrators.
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ApiResponse<ActivityLogSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetActivityLogSummary()
    {
        var result = await _auditService.GetActivityLogSummaryAsync();
        return Success(result, "Activity log summary retrieved successfully");
    }

    /// <summary>
    /// Retrieves the historical lifecycle timeline of a specific entity (e.g. Enrollment, Payment, Track).
    /// </summary>
    [HttpGet("timeline/{entityName}/{entityId}")]
    [ProducesResponseType(typeof(ApiResponse<List<ActivityLogListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetEntityTimeline(string entityName, string entityId)
    {
        var result = await _auditService.GetEntityTimelineAsync(entityName, entityId);
        return Success(result, $"Timeline for {entityName} #{entityId} retrieved successfully");
    }
}
