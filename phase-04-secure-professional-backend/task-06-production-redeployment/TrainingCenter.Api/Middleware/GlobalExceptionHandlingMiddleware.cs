using System.Net;
using System.Text.Json;
using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.Common.Responses;

namespace TrainingCenter.Api.Middleware;

/// <summary>
/// Global exception handling middleware that catches domain exceptions and unexpected server faults.
/// Formats errors into a unified ApiResponse object structure and shields internal details in production.
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            _logger.LogWarning("Domain application exception occurred: {Message} (StatusCode: {StatusCode})", ex.Message, ex.StatusCode);
            await HandleExceptionAsync(context, ex.StatusCode, ex.Message, ex.Errors);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
        {
            _logger.LogWarning(dbEx, "Database update constraint conflict occurred during request processing.");
            var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
            var errors = _env.IsDevelopment() ? new List<string> { innerMessage } : new List<string> { "A database constraint conflict occurred while persisting changes." };
            await HandleExceptionAsync(context, (int)HttpStatusCode.Conflict, "Database operation conflict. The record may already exist or violate relational integrity.", errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled system exception occurred during request pipeline processing.");

            var errorList = new List<string>();
            if (_env.IsDevelopment())
            {
                errorList.Add(ex.Message);
                if (ex.InnerException != null)
                {
                    errorList.Add(ex.InnerException.Message);
                }
            }
            else
            {
                errorList.Add("An internal error occurred. Please contact system support with your correlation ID.");
            }

            await HandleExceptionAsync(
                context,
                (int)HttpStatusCode.InternalServerError,
                "An unexpected internal error occurred on the server.",
                errorList);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, int statusCode, string message, List<string> errors)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = ApiResponse<object>.FailureResponse(message, errors, statusCode);
        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return context.Response.WriteAsync(jsonResponse);
    }
}
