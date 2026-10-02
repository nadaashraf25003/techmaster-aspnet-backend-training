using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common;
using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.Data;
using TrainingCenter.Api.DTOs.Auth;
using TrainingCenter.Api.Entities;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly TrainingCenterDbContext _context;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        TrainingCenterDbContext context,
        IPasswordHasherService passwordHasher,
        IJwtTokenService jwtTokenService,
        ILogger<AuthService> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    public async Task<UserSummaryDto> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Role == UserRole.Admin)
        {
            throw new BadRequestException("Registering as an Admin via public registration is not permitted. Contact system administrator.");
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var emailExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email.ToLower() == normalizedEmail, cancellationToken);

        if (emailExists)
        {
            throw new ConflictException($"A user with email '{request.Email}' is already registered.");
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var nowUtc = DateTime.UtcNow;

        var user = new ApplicationUser
        {
            FullName = request.FullName.Trim(),
            Email = normalizedEmail,
            PasswordHash = passwordHash,
            Role = request.Role,
            IsActive = true,
            CreatedAtUtc = nowUtc
        };

        // Automatically provision and link domain profile based on role
        if (request.Role == UserRole.Student)
        {
            var student = new Student
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                DateOfBirth = DateTime.UtcNow.AddYears(-20), // Default placeholder
                IsActive = true,
                CreatedAt = nowUtc
            };
            _context.Students.Add(student);
            user.Student = student;
        }
        else if (request.Role == UserRole.Instructor)
        {
            var instructor = new Instructor
            {
                FullName = user.FullName,
                Email = user.Email,
                Specialization = string.IsNullOrWhiteSpace(request.Specialization) ? "Software Engineering" : request.Specialization.Trim(),
                HourlyRate = 50.00m,
                IsActive = true,
                CreatedAt = nowUtc
            };
            _context.Instructors.Add(instructor);
            user.Instructor = instructor;
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("New user registered successfully: UserId={UserId}, Email={Email}, Role={Role}",
            user.Id, user.Email, user.Role);

        return new UserSummaryDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            LinkedStudentId = user.StudentId,
            LinkedInstructorId = user.InstructorId,
            CreatedAtUtc = user.CreatedAtUtc
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await _context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Failed login attempt for non-existent email: {Email}", request.Email);
            throw new UnauthorizedException("Invalid email or password.");
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("Login rejected for deactivated user: UserId={UserId}, Email={Email}", user.Id, user.Email);
            throw new ForbiddenException("Your account has been deactivated. Please contact TechMaster support.");
        }

        var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            _logger.LogWarning("Failed login attempt due to invalid password: UserId={UserId}, Email={Email}", user.Id, user.Email);
            throw new UnauthorizedException("Invalid email or password.");
        }

        // Generate JWT and Refresh Token
        var (accessToken, expiresAt) = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken(ipAddress);

        user.AddRefreshToken(refreshToken);
        user.UpdateLastLogin(DateTime.UtcNow);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User logged in successfully: UserId={UserId}, Email={Email}, Role={Role}",
            user.Id, user.Email, user.Role);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            LinkedStudentId = user.StudentId,
            LinkedInstructorId = user.InstructorId
        };
    }

    public async Task<CurrentUserResponse> GetCurrentUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException($"User with ID {userId} was not found.");
        }

        return new CurrentUserResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            LinkedStudentId = user.StudentId,
            LinkedInstructorId = user.InstructorId,
            LastLoginAt = user.LastLoginAtUtc
        };
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {userId} was not found.");
        }

        if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            throw new BadRequestException("Current password does not match our records.");
        }

        user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
        user.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Password successfully changed for UserId={UserId}", userId);
        return true;
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == request.RefreshToken), cancellationToken);

        if (user == null)
        {
            throw new UnauthorizedException("Invalid refresh token.");
        }

        var existingToken = user.RefreshTokens.FirstOrDefault(t => t.Token == request.RefreshToken);
        if (existingToken == null || !existingToken.IsActive)
        {
            throw new UnauthorizedException("Refresh token is expired or revoked.");
        }

        // Generate new tokens
        var (newAccessToken, expiresAt) = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken(ipAddress);

        user.RevokeRefreshToken(existingToken.Token, ipAddress, "Replaced by new token", newRefreshToken.Token);
        user.AddRefreshToken(newRefreshToken);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Refresh token rotated for UserId={UserId}", user.Id);

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            LinkedStudentId = user.StudentId,
            LinkedInstructorId = user.InstructorId
        };
    }

    public async Task<bool> LogoutAsync(int userId, string? refreshToken, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            return true;
        }

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            user.RevokeRefreshToken(refreshToken, ipAddress, "Explicit user logout");
        }
        else
        {
            // Revoke all active tokens for this user
            foreach (var token in user.RefreshTokens.Where(t => t.IsActive))
            {
                token.RevokedAtUtc = DateTime.UtcNow;
                token.RevokedByIp = ipAddress;
                token.ReasonRevoked = "All-session logout";
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("User logged out: UserId={UserId}", userId);
        return true;
    }
}
