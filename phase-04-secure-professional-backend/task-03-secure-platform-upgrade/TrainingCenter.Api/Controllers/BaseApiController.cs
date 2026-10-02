using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.DTOs.Common;

namespace TrainingCenter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult Success<T>(T data, string message = "Success", int statusCode = StatusCodes.Status200OK)
    {
        var response = ApiResponse<T>.SuccessResponse(data, message, statusCode);
        return StatusCode(statusCode, response);
    }

    protected IActionResult CreatedSuccess<T>(string actionName, object? routeValues, T data, string message = "Resource created successfully")
    {
        var response = ApiResponse<T>.SuccessResponse(data, message, StatusCodes.Status201Created);
        return CreatedAtAction(actionName, routeValues, response);
    }

    protected IActionResult CreatedSuccess<T>(string uri, T data, string message = "Resource created successfully")
    {
        var response = ApiResponse<T>.SuccessResponse(data, message, StatusCodes.Status201Created);
        return Created(uri, response);
    }

    protected int GetAuthenticatedUserId()
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? User.FindFirstValue(ClaimTypes.Name);

        if (int.TryParse(claimValue, out var userId))
        {
            return userId;
        }

        return 0;
    }

    protected string? GetAuthenticatedUserRole()
    {
        return User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role");
    }

    protected int? GetAuthenticatedStudentId()
    {
        var claimValue = User.FindFirstValue("studentId");
        if (int.TryParse(claimValue, out var studentId))
        {
            return studentId;
        }
        return null;
    }

    protected int? GetAuthenticatedInstructorId()
    {
        var claimValue = User.FindFirstValue("instructorId");
        if (int.TryParse(claimValue, out var instructorId))
        {
            return instructorId;
        }
        return null;
    }

    protected bool IsAdmin() => User.IsInRole("Admin");
    protected bool IsInstructor() => User.IsInRole("Instructor");
    protected bool IsStudent() => User.IsInRole("Student");

    protected string? GetClientIpAddress()
    {
        if (Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            return forwardedFor.FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim();
        }

        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
