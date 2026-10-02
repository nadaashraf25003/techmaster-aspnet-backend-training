using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.DTOs.Auth;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Controllers;

/// <summary>
/// Authentication and identity lifecycle management endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registers a new user account (Student or Instructor) and provisions their profile.
    /// </summary>
    /// <param name="request">Registration payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="201">User registered successfully.</response>
    /// <response code="400">Validation error or attempt to register as Admin.</response>
    /// <response code="409">Email address is already in use.</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<UserSummaryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        return CreatedSuccess(nameof(GetCurrentUser), null, result, "User registered successfully.");
    }

    /// <summary>
    /// Authenticates a user with email and password, issuing a JWT access token and refresh token.
    /// </summary>
    /// <param name="request">Login credentials.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Authentication successful; returns JWT and user identity.</response>
    /// <response code="400">Invalid request format.</response>
    /// <response code="401">Invalid email or password.</response>
    /// <response code="403">User account is deactivated.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var ipAddress = GetClientIpAddress();
        var result = await _authService.LoginAsync(request, ipAddress, cancellationToken);
        return Success(result, "Authentication successful.");
    }

    /// <summary>
    /// Retrieves identity, claims, and profile linkage for the currently authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns current authenticated user details.</response>
    /// <response code="401">Unauthorized; missing or invalid JWT bearer token.</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<CurrentUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();
        var result = await _authService.GetCurrentUserAsync(userId, cancellationToken);
        return Success(result, "Current user retrieved successfully.");
    }

    /// <summary>
    /// Changes the password of the currently authenticated user.
    /// </summary>
    /// <param name="request">Old and new passwords.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Password changed successfully.</response>
    /// <response code="400">Incorrect current password or invalid new password format.</response>
    /// <response code="401">Unauthorized.</response>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();
        var result = await _authService.ChangePasswordAsync(userId, request, cancellationToken);
        return Success(result, "Password changed successfully.");
    }

    /// <summary>
    /// Rotates a refresh token to issue a renewed JWT access token and new refresh token.
    /// </summary>
    /// <param name="request">Refresh token payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Token refreshed successfully.</response>
    /// <response code="401">Invalid, expired, or revoked refresh token.</response>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var ipAddress = GetClientIpAddress();
        var result = await _authService.RefreshTokenAsync(request, ipAddress, cancellationToken);
        return Success(result, "Token refreshed successfully.");
    }

    /// <summary>
    /// Revokes the current refresh token and logs out the authenticated user.
    /// </summary>
    /// <param name="request">Optional specific refresh token to revoke.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Logged out successfully.</response>
    /// <response code="401">Unauthorized.</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout([FromBody] RevokeTokenRequest? request, CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();
        var ipAddress = GetClientIpAddress();
        var result = await _authService.LogoutAsync(userId, request?.RefreshToken, ipAddress, cancellationToken);
        return Success(result, "Logged out successfully.");
    }
}
