using System.Security.Claims;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Data;
using TrainingCenter.Api.DTOs.ActivityLogs;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.Entities;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Services.Implementations;

public class AuditService : IAuditService
{
    private readonly TrainingCenterDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuditService> _logger;

    public AuditService(
        TrainingCenterDbContext context,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuditService> logger)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task LogAsync(
        string action,
        string entityName,
        string? entityId,
        string description,
        object? metadata = null,
        int? userId = null,
        string? userRole = null,
        string? userEmail = null)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;

            // Extract identity from HttpContext if not passed explicitly
            if (!userId.HasValue && httpContext?.User.Identity?.IsAuthenticated == true)
            {
                var idClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(idClaim, out var parsedId))
                {
                    userId = parsedId;
                }
            }

            if (string.IsNullOrWhiteSpace(userRole) && httpContext?.User.Identity?.IsAuthenticated == true)
            {
                userRole = httpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            }

            if (string.IsNullOrWhiteSpace(userEmail) && httpContext?.User.Identity?.IsAuthenticated == true)
            {
                userEmail = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            }

            var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString();
            var correlationId = httpContext?.Response.Headers["X-Correlation-ID"].ToString();
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                correlationId = httpContext?.Request.Headers["X-Correlation-ID"].ToString();
            }

            string? metadataJson = null;
            if (metadata != null)
            {
                metadataJson = metadata is string strMeta ? strMeta : JsonSerializer.Serialize(metadata, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                });
            }

            var logEntry = new ActivityLog
            {
                UserId = userId,
                UserRole = userRole,
                UserEmail = userEmail,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                IpAddress = ipAddress,
                CorrelationId = correlationId,
                Metadata = metadataJson
            };

            await _context.ActivityLogs.AddAsync(logEntry);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Audit Logged: [{Action}] on {EntityName}:{EntityId} by User {UserId} ({UserRole}) - {Description}",
                action, entityName, entityId ?? "N/A", userId?.ToString() ?? "Anonymous", userRole ?? "N/A", description);
        }
        catch (Exception ex)
        {
            // Fail-safe: Audit failure should not crash the primary business operation
            _logger.LogError(ex, "Failed to persist audit log entry for action {Action} on {EntityName}", action, entityName);
        }
    }

    public async Task<PagedResult<ActivityLogListItemResponse>> GetActivityLogsAsync(ActivityLogFilterParams filters)
    {
        var query = _context.ActivityLogs
            .AsNoTracking()
            .AsQueryable();

        if (filters.UserId.HasValue)
        {
            query = query.Where(a => a.UserId == filters.UserId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.EntityName))
        {
            var entity = filters.EntityName.Trim().ToLower();
            query = query.Where(a => a.EntityName.ToLower() == entity);
        }

        if (!string.IsNullOrWhiteSpace(filters.Action))
        {
            var act = filters.Action.Trim().ToLower();
            query = query.Where(a => a.Action.ToLower() == act);
        }

        if (filters.From.HasValue)
        {
            var fromUtc = filters.From.Value.Date;
            query = query.Where(a => a.CreatedAt >= fromUtc);
        }

        if (filters.To.HasValue)
        {
            var toUtc = filters.To.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(a => a.CreatedAt <= toUtc);
        }

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var search = filters.Search.Trim().ToLower();
            query = query.Where(a =>
                a.Description.ToLower().Contains(search) ||
                (a.UserEmail != null && a.UserEmail.ToLower().Contains(search)) ||
                (a.Action != null && a.Action.ToLower().Contains(search)) ||
                (a.EntityId != null && a.EntityId.ToLower().Contains(search)));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((filters.PageNumber - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .Select(a => new ActivityLogListItemResponse
            {
                ActivityLogId = a.ActivityLogId,
                UserId = a.UserId,
                UserRole = a.UserRole,
                UserEmail = a.UserEmail,
                Action = a.Action,
                EntityName = a.EntityName,
                EntityId = a.EntityId,
                Description = a.Description,
                CreatedAt = a.CreatedAt,
                IpAddress = a.IpAddress,
                CorrelationId = a.CorrelationId,
                Metadata = a.Metadata
            })
            .ToListAsync();

        return new PagedResult<ActivityLogListItemResponse>(items, totalCount, filters.PageNumber, filters.PageSize);
    }

    public async Task<ActivityLogListItemResponse?> GetActivityLogByIdAsync(int id)
    {
        var log = await _context.ActivityLogs
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.ActivityLogId == id);

        if (log == null) return null;

        return new ActivityLogListItemResponse
        {
            ActivityLogId = log.ActivityLogId,
            UserId = log.UserId,
            UserRole = log.UserRole,
            UserEmail = log.UserEmail,
            Action = log.Action,
            EntityName = log.EntityName,
            EntityId = log.EntityId,
            Description = log.Description,
            CreatedAt = log.CreatedAt,
            IpAddress = log.IpAddress,
            CorrelationId = log.CorrelationId,
            Metadata = log.Metadata
        };
    }

    public async Task<ActivityLogSummaryResponse> GetActivityLogSummaryAsync()
    {
        var totalLogs = await _context.ActivityLogs.CountAsync();
        var totalUsersAudited = await _context.ActivityLogs
            .Where(a => a.UserId.HasValue)
            .Select(a => a.UserId!.Value)
            .Distinct()
            .CountAsync();

        var actionCounts = await _context.ActivityLogs
            .GroupBy(a => a.Action)
            .Select(g => new { Action = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Action, x => x.Count);

        var entityCounts = await _context.ActivityLogs
            .GroupBy(a => a.EntityName)
            .Select(g => new { Entity = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Entity, x => x.Count);

        var recentActivities = await _context.ActivityLogs
            .AsNoTracking()
            .OrderByDescending(a => a.CreatedAt)
            .Take(10)
            .Select(a => new ActivityLogListItemResponse
            {
                ActivityLogId = a.ActivityLogId,
                UserId = a.UserId,
                UserRole = a.UserRole,
                UserEmail = a.UserEmail,
                Action = a.Action,
                EntityName = a.EntityName,
                EntityId = a.EntityId,
                Description = a.Description,
                CreatedAt = a.CreatedAt,
                IpAddress = a.IpAddress,
                CorrelationId = a.CorrelationId,
                Metadata = a.Metadata
            })
            .ToListAsync();

        return new ActivityLogSummaryResponse
        {
            TotalLogs = totalLogs,
            TotalUsersAudited = totalUsersAudited,
            ActionBreakdown = actionCounts,
            EntityBreakdown = entityCounts,
            RecentActivities = recentActivities
        };
    }

    public async Task<List<ActivityLogListItemResponse>> GetEntityTimelineAsync(string entityName, string entityId)
    {
        return await _context.ActivityLogs
            .AsNoTracking()
            .Where(a => a.EntityName.ToLower() == entityName.ToLower() && a.EntityId == entityId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new ActivityLogListItemResponse
            {
                ActivityLogId = a.ActivityLogId,
                UserId = a.UserId,
                UserRole = a.UserRole,
                UserEmail = a.UserEmail,
                Action = a.Action,
                EntityName = a.EntityName,
                EntityId = a.EntityId,
                Description = a.Description,
                CreatedAt = a.CreatedAt,
                IpAddress = a.IpAddress,
                CorrelationId = a.CorrelationId,
                Metadata = a.Metadata
            })
            .ToListAsync();
    }
}
