using TrainingCenter.Api.Constants;

namespace TrainingCenter.Api.Middleware;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(AuthConstants.CorrelationIdHeader, out var correlationId) ||
            string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString("N");
        }

        context.Items[AuthConstants.CorrelationIdHeader] = correlationId.ToString();
        context.Response.Headers[AuthConstants.CorrelationIdHeader] = correlationId.ToString();

        await _next(context);
    }
}
