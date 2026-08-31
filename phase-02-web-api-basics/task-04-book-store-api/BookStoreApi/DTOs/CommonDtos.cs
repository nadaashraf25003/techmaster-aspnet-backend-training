namespace BookStoreApi.DTOs;

public class ApiErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class ApiResponse
{
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; } = true;
}
