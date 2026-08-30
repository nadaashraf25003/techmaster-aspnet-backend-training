namespace StudentManagementApi.DTOs;

/// <summary>
/// Response returned when student status is modified.
/// </summary>
public class StudentStatusResponseDto
{
    public string Message { get; set; } = string.Empty;
    public StudentResponseDto Student { get; set; } = null!;
}

/// <summary>
/// General API error response object.
/// </summary>
public class ApiErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
