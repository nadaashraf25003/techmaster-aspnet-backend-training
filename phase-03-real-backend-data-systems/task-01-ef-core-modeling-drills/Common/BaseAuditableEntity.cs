namespace EFCoreModelingDrills.Common;

/// <summary>
/// Base entity providing audit fields and soft-delete capabilities.
/// </summary>
public abstract class BaseAuditableEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}
