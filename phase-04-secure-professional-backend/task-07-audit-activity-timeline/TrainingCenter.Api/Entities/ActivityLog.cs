namespace TrainingCenter.Api.Entities;

/// <summary>
/// Immutable audit log capturing critical business actions, state mutations, and security events.
/// </summary>
public class ActivityLog
{
    public int ActivityLogId { get; set; }

    public int? UserId { get; set; }
    public string? UserRole { get; set; }
    public string? UserEmail { get; set; }

    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string? EntityId { get; set; }

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? IpAddress { get; set; }
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Structured JSON metadata storing change differentials (e.g. oldStatus, newStatus, amounts).
    /// </summary>
    public string? Metadata { get; set; }
}
