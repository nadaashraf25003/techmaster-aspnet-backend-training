using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.DTOs.Common;

namespace TrainingCenter.Api.DTOs.ActivityLogs;

public class ActivityLogFilterParams : PaginationParams
{
    [FromQuery(Name = "userId")]
    public int? UserId { get; set; }

    [FromQuery(Name = "entityName")]
    public string? EntityName { get; set; }

    [FromQuery(Name = "action")]
    public string? Action { get; set; }

    [FromQuery(Name = "from")]
    public DateTime? From { get; set; }

    [FromQuery(Name = "to")]
    public DateTime? To { get; set; }

    [FromQuery(Name = "search")]
    public string? Search { get; set; }
}

public class ActivityLogListItemResponse
{
    public int ActivityLogId { get; set; }
    public int? UserId { get; set; }
    public string? UserRole { get; set; }
    public string? UserEmail { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? IpAddress { get; set; }
    public string? CorrelationId { get; set; }
    public string? Metadata { get; set; }
}

public class ActivityLogSummaryResponse
{
    public int TotalLogs { get; set; }
    public int TotalUsersAudited { get; set; }
    public Dictionary<string, int> ActionBreakdown { get; set; } = new();
    public Dictionary<string, int> EntityBreakdown { get; set; } = new();
    public List<ActivityLogListItemResponse> RecentActivities { get; set; } = new();
}
