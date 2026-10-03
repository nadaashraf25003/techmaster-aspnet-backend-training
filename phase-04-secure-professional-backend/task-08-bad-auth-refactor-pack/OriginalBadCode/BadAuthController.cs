using Microsoft.AspNetCore.Mvc;

namespace TrainingCenter.Api.OriginalBadCode;

// =========================================================================================================
// ⚠️ VULNERABLE / INSECURE CODE ARTIFACT - FOR EDUCATIONAL REFACTORING AUDIT ONLY
// =========================================================================================================
// This file represents the exact unrefactored legacy code containing critical security vulnerabilities,
// architectural anti-patterns, missing input validation, improper HTTP status codes, and plaintext storage.
// =========================================================================================================

/*
 * CRITICAL CODE REVIEW FINDINGS & VULNERABILITIES IDENTIFIED:
 * 
 * 1. [CRITICAL SECURITY] Plaintext Password Storage & Comparison:
 *    - user.PasswordHash == request.Password stores and verifies passwords without cryptographic hashing.
 *    - Passwords will leak immediately in database dumps or query logs.
 *    - FIX: Use PBKDF2 / Argon2 / ASP.NET Core PasswordHasher with random salt.
 * 
 * 2. [CRITICAL SECURITY] Token Forgery & Fake Token Generation:
 *    - var token = "fake-token-" + user.Id generates a predictable, forgeable token.
 *    - Any attacker can impersonate user ID 1 (Admin) by sending "fake-token-1".
 *    - FIX: Generate cryptographically signed JWT with HMAC-SHA256 and verified claims.
 * 
 * 3. [INFORMATION DISCLOSURE / OVER-POSTING] Full User Entity Exposure:
 *    - return Ok(new { token = token, user = user }) and return Ok(user) returns the entire DB entity.
 *    - Exposes sensitive database fields like PasswordHash, SecurityStamp, internal IDs, and deleted flags.
 *    - FIX: Map to strongly-typed AuthResponse and UserSummaryDto without sensitive properties.
 * 
 * 4. [INPUT VALIDATION] Missing Request Validation & Sanity Checks:
 *    - No check for empty or invalid email formats, weak passwords, or missing fields.
 *    - No duplicate email check in Register (causes unhandled SqlException / duplicate key crash).
 *    - FIX: Add DataAnnotations / FluentValidation, regex email validation, and duplicate conflict checks.
 * 
 * 5. [SECURITY / PRIVILEGE ESCALATION] Unrestricted Role Assignment:
 *    - user.Role = request.Role allows any anonymous caller to register as an "Admin".
 *    - FIX: Prohibit public Admin self-registration; enforce Student/Instructor defaults or admin-only approval.
 * 
 * 6. [ARCHITECTURE] Direct DbContext Coupling in Controller:
 *    - Controller directly manipulates AppDbContext, violating Single Responsibility Principle (SRP)
 *      and Layered Architecture principles.
 *    - FIX: Extract domain business logic into IAuthService / AuthService with dependency injection.
 * 
 * 7. [INCORRECT HTTP STATUS CODES] Silent 200 OK Failures:
 *    - Returns HTTP 200 OK with "wrong email" or "wrong password" strings instead of 401 Unauthorized / 400 Bad Request.
 *    - Breaks RESTful API contracts and confuses API client error-handling libraries.
 *    - FIX: Return standard HTTP 400, 401, 403, and 409 responses with unified ApiResponse<T> envelope.
 * 
 * 8. [PERFORMANCE / SCALABILITY] Blocking Synchronous Database Execution:
 *    - Uses db.Users.FirstOrDefault(...) and db.SaveChanges() instead of asynchronous counterparts.
 *    - Blocks ASP.NET Core thread pool threads under heavy load, causing thread starvation.
 *    - FIX: Use FirstOrDefaultAsync and SaveChangesAsync with CancellationToken support.
 * 
 * 9. [BUSINESS RULE / ACCESS CONTROL] Missing Account Status Check (IsActive):
 *    - Inactive or banned users can continue to log in without restriction.
 *    - FIX: Verify user.IsActive before generating tokens and return 403 Forbidden for deactivated accounts.
 * 
 * 10. [AUDIT & OBSERVABILITY] Lack of Structured Logging & Audit Trail:
 *     - No logging for security events (login attempts, failed logins, registration).
 *     - Impossible to perform forensic investigation or detect brute force / credential stuffing attacks.
 *     - FIX: Inject ILogger and IAuditService to capture login/registration events with IP and correlation IDs.
 */

[ApiController]
[Route("api/auth")]
public class BadAuthController : ControllerBase
{
    // Anti-Pattern: Direct DbContext reference in controller
    private readonly dynamic db;

    public BadAuthController(dynamic db) 
    { 
        this.db = db; 
    }

    [HttpPost("login")]
    public IActionResult Login(BadLoginRequest request)
    {
        // Anti-Pattern 1: Synchronous call
        // Anti-Pattern 2: Missing email normalization
        var user = db.Users.FirstOrDefault((System.Func<dynamic, bool>)(x => x.Email == request.Email));

        // Anti-Pattern 3: Returns 200 OK for failure with raw string
        if (user == null) 
            return Ok("wrong email");

        // Anti-Pattern 4: Plaintext password comparison
        if (user.PasswordHash != request.Password) 
            return Ok("wrong password");

        // Anti-Pattern 5: Fake, unauthenticated token string without cryptographic signature
        var token = "fake-token-" + user.Id;

        // Anti-Pattern 6: Over-exposing user entity with password hash
        return Ok(new { token = token, user = user });
    }

    [HttpPost("register")]
    public IActionResult Register(BadRegisterRequest request)
    {
        // Anti-Pattern 7: No duplicate email check
        // Anti-Pattern 8: Privilege escalation (client specifies Admin role)
        // Anti-Pattern 9: Plaintext password saved directly as hash
        var user = new
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = request.Password,
            Role = request.Role
        };

        db.Users.Add(user);
        db.SaveChanges(); // Anti-Pattern 10: Synchronous blocking save

        // Anti-Pattern 11: Returning raw entity
        return Ok(user);
    }
}

public class BadLoginRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class BadRegisterRequest
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; }
}
