using System.Security.Claims;
using TrainingCenter.Api.Entities;

namespace TrainingCenter.Api.Services.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateAccessToken(ApplicationUser user);
    RefreshToken GenerateRefreshToken(string? ipAddress);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
