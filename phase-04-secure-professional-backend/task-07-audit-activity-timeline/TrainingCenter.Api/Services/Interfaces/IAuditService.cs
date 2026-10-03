using TrainingCenter.Api.DTOs.ActivityLogs;
using TrainingCenter.Api.DTOs.Common;

namespace TrainingCenter.Api.Services.Interfaces;

public interface IAuditService
{
    /// <summary>
    /// Records an immutable audit log entry into the database.
    /// Extracts caller identity, IP, and CorrelationId from HTTP context when available.
    /// </summary>
    Task LogAsync(
        string action,
        string entityName,
        string? entityId,
        string description,
        object? metadata = null,
        int? userId = null,
        string? userRole = null,
        string? userEmail = null);

    Task<PagedResult<ActivityLogListItemResponse>> GetActivityLogsAsync(ActivityLogFilterParams filters);

    Task<ActivityLogListItemResponse?> GetActivityLogByIdAsync(int id);

    Task<ActivityLogSummaryResponse> GetActivityLogSummaryAsync();

    Task<List<ActivityLogListItemResponse>> GetEntityTimelineAsync(string entityName, string entityId);
}
