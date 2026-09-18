using System.Net;
using System.Text.Json;
using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.DTOs.Common;

namespace TrainingCenter.Api.Common.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate _next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        this._next = _next;
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
            _logger.LogError(ex, "Unhandled Exception caught in API Middleware: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        int statusCode;
        string message;
        List<string>? errors = null;

        switch (exception)
        {
            case AppException appEx:
                statusCode = appEx.StatusCode;
                message = appEx.Message;
                errors = appEx.Errors;
                break;
            case KeyNotFoundException:
                statusCode = (int)HttpStatusCode.NotFound;
                message = "The requested resource was not found.";
                break;
            case ArgumentException argEx:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = argEx.Message;
                break;
            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = "An unexpected server error occurred. Please try again later.";
                break;
        }

        context.Response.StatusCode = statusCode;

        var response = ApiResponse<object>.FailureResponse(message, errors, statusCode);
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}
