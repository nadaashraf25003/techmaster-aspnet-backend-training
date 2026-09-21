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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception caught by global middleware: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, errors) = exception switch
        {
            NotFoundException notFound => (
                HttpStatusCode.NotFound,
                notFound.Message,
                new List<string> { notFound.Message }
            ),
            ConflictException conflict => (
                HttpStatusCode.Conflict,
                conflict.Message,
                new List<string> { conflict.Message }
            ),
            BusinessRuleException businessRule => (
                HttpStatusCode.BadRequest,
                businessRule.Message,
                new List<string> { businessRule.Message }
            ),
            ValidationException validation => (
                HttpStatusCode.BadRequest,
                validation.Message,
                new List<string> { validation.Message }
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                "An unexpected server error occurred. Please contact support.",
                new List<string> { exception.Message }
            )
        };

        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.FailureResponse(message, errors, (int)statusCode);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        return context.Response.WriteAsync(json);
    }
}
