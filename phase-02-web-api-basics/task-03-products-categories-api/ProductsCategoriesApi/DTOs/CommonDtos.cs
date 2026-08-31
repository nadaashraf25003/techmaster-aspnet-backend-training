namespace ProductsCategoriesApi.DTOs;

/// <summary>
/// Standard API error response.
/// </summary>
public class ApiErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// General message response for status or delete operations.
/// </summary>
public class ApiResponse
{
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; } = true;
}
