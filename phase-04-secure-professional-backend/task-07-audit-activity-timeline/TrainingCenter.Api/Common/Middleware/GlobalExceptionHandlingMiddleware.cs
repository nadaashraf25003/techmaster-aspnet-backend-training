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
            _logger.LogWarning("Domain application exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex.StatusCode, ex.Message, ex.Errors);
        }
        catch (Exception ex)
        {
            var errorList = new List<string> { ex.Message };
            if (ex.InnerException != null)
            {
                errorList.Add(ex.InnerException.Message);
                if (ex.InnerException.InnerException != null)
                {
                    errorList.Add(ex.InnerException.InnerException.Message);
                }
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
