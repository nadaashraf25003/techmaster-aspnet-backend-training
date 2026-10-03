using System.Text.Json.Serialization;

namespace TrainingCenter.Api.Common.Responses;

/// <summary>
/// Unified standard response envelope for all API endpoints.
/// Guarantees consistent JSON structure across success and failure outcomes.
/// </summary>
/// <typeparam name="T">The payload data type.</typeparam>
public class ApiResponse<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("errors")]
    public List<string> Errors { get; set; } = new();

    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public ApiResponse()
    {
    }

    public ApiResponse(bool success, string message, T? data, List<string>? errors = null, int statusCode = 200)
    {
        Success = success;
        Message = message;
        Data = data;
        Errors = errors ?? new List<string>();
        StatusCode = statusCode;
        Timestamp = DateTime.UtcNow;
    }

    public static ApiResponse<T> SuccessResponse(T data, string message = "Success", int statusCode = 200)
    {
        return new ApiResponse<T>(true, message, data, null, statusCode);
    }

    public static ApiResponse<T> FailureResponse(string message, List<string>? errors = null, int statusCode = 400)
    {
        return new ApiResponse<T>(false, message, default, errors, statusCode);
    }
}
