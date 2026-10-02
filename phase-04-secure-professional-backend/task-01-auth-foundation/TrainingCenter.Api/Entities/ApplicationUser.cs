using TrainingCenter.Api.Common;

namespace TrainingCenter.Api.Entities;

public class ApplicationUser : BaseAuditableEntity
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Student;
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAtUtc { get; set; }

    public DateTime CreatedAtUtc
    {
        get => CreatedAt;
        set => CreatedAt = value;
    }

    public DateTime? UpdatedAtUtc
    {
        get => LastModifiedAt;
        set => LastModifiedAt = value;
    }

    // Optional 1-to-1 links to domain profiles
    public int? StudentId { get; set; }
    public virtual Student? Student { get; set; }

    public int? InstructorId { get; set; }
    public virtual Instructor? Instructor { get; set; }

    // Refresh Tokens for JWT session management
    private readonly List<RefreshToken> _refreshTokens = new();
    public virtual IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    public void UpdateLastLogin(DateTime loginTimeUtc)
    {
        LastLoginAtUtc = loginTimeUtc;
        LastModifiedAt = loginTimeUtc;
    }

    public void AddRefreshToken(RefreshToken token)
    {
        _refreshTokens.Add(token);
    }

    public void RevokeRefreshToken(string token, string? ipAddress, string? reason = null, string? replacedByToken = null)
    {
        var targetToken = _refreshTokens.FirstOrDefault(t => t.Token == token && t.IsActive);
        if (targetToken != null)
        {
            targetToken.RevokedAtUtc = DateTime.UtcNow;
            targetToken.RevokedByIp = ipAddress;
            targetToken.ReasonRevoked = reason ?? "Revoked by user request";
            targetToken.ReplacedByToken = replacedByToken;
        }
    }
}
