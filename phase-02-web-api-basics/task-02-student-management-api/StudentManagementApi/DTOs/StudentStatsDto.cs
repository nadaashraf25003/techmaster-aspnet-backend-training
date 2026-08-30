namespace StudentManagementApi.DTOs;

/// <summary>
/// Aggregated student statistics for academy management.
/// </summary>
public class StudentStatsDto
{
    public int TotalStudents { get; set; }
    public int ActiveStudents { get; set; }
    public int InactiveStudents { get; set; }
    public Dictionary<string, int> CountByTrack { get; set; } = new();
}
