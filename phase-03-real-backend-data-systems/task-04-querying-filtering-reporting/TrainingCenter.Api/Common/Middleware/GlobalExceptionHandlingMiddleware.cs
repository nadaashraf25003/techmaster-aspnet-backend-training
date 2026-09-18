using System.Net;
using System.Text.Json;
using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.DTOs.Common;

namespace TrainingCenter.Api.Common.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            _logger.LogWarning("Handled domain exception: {Message} with status {StatusCode}", ex.Message, ex.StatusCode);
            await HandleExceptionAsync(context, ex.StatusCode, ex.Message, ex.Errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled server error occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, (int)HttpStatusCode.InternalServerError, "An unexpected internal server error occurred.", new List<string> { ex.Message });
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, int statusCode, string message, List<string>? errors = null)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = ApiResponse<object>.FailureResponse(message, errors ?? new List<string>(), statusCode);

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }
}
