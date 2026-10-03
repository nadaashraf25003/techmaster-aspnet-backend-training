using TrainingCenter.Api.DTOs.Auth;

namespace TrainingCenter.Api.Services.Interfaces;

public interface IAuthService
{
    Task<UserSummaryDto> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken cancellationToken = default);
    Task<CurrentUserResponse> GetCurrentUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, string? ipAddress, CancellationToken cancellationToken = default);
    Task<bool> LogoutAsync(int userId, string? refreshToken, string? ipAddress, CancellationToken cancellationToken = default);
}
