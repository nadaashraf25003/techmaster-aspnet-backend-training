using System.Security.Claims;
using TrainingCenter.Api.Constants;

namespace TrainingCenter.Api.Helpers;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal? principal)
    {
        if (principal == null) return 0;

        var claimValue = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal.FindFirstValue("sub")
            ?? principal.FindFirstValue(ClaimTypes.Name);

        return int.TryParse(claimValue, out var userId) ? userId : 0;
    }

    public static string? GetUserRole(this ClaimsPrincipal? principal)
    {
        if (principal == null) return null;
        return principal.FindFirstValue(ClaimTypes.Role) ?? principal.FindFirstValue(AuthConstants.RoleClaim);
    }

    public static int? GetStudentId(this ClaimsPrincipal? principal)
    {
        if (principal == null) return null;

        var claimValue = principal.FindFirstValue(AuthConstants.StudentIdClaim);
        return int.TryParse(claimValue, out var studentId) ? studentId : null;
    }

    public static int? GetInstructorId(this ClaimsPrincipal? principal)
    {
        if (principal == null) return null;

        var claimValue = principal.FindFirstValue(AuthConstants.InstructorIdClaim);
        return int.TryParse(claimValue, out var instructorId) ? instructorId : null;
    }

    public static bool IsAdmin(this ClaimsPrincipal? principal) => principal?.IsInRole(AppRoles.Admin) ?? false;
    public static bool IsInstructor(this ClaimsPrincipal? principal) => principal?.IsInRole(AppRoles.Instructor) ?? false;
    public static bool IsStudent(this ClaimsPrincipal? principal) => principal?.IsInRole(AppRoles.Student) ?? false;
}
