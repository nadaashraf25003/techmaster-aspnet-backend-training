using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.Common.Responses;
using TrainingCenter.Api.Constants;
using TrainingCenter.Api.Helpers;

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
        return StatusCode(StatusCodes.Status201Created, response);
    }

    protected IActionResult CreatedSuccess<T>(T data, string message = "Resource created successfully")
    {
        var response = ApiResponse<T>.SuccessResponse(data, message, StatusCodes.Status201Created);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Success(result.Value, result.Message, result.StatusCode);
        }

        var failureResponse = ApiResponse<T>.FailureResponse(result.Message, result.Errors, result.StatusCode);
        return StatusCode(result.StatusCode, failureResponse);
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Success<object?>(null, result.Message, result.StatusCode);
        }

        var failureResponse = ApiResponse<object>.FailureResponse(result.Message, result.Errors, result.StatusCode);
        return StatusCode(result.StatusCode, failureResponse);
    }

    protected int GetAuthenticatedUserId() => User.GetUserId();
    protected string? GetAuthenticatedUserRole() => User.GetUserRole();
    protected int? GetAuthenticatedStudentId() => User.GetStudentId();
    protected int? GetAuthenticatedInstructorId() => User.GetInstructorId();

    protected bool IsAdmin() => User.IsAdmin();
    protected bool IsInstructor() => User.IsInstructor();
    protected bool IsStudent() => User.IsStudent();

    protected string? GetClientIpAddress()
    {
        if (Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            return forwardedFor.FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim();
        }

        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
