namespace TrainingCenter.Api.DTOs.Reports;

public class DashboardSummaryDto
{
    public int StudentsCount { get; set; }
    public int ActiveStudentsCount { get; set; }
    public int InstructorsCount { get; set; }
    public int TracksCount { get; set; }
    public int ActiveTracksCount { get; set; }
    public int TotalEnrollmentsCount { get; set; }
    public int ActiveEnrollments { get; set; }
    public decimal Revenue { get; set; }
    public decimal RealizedRevenue { get; set; }
    public decimal ExpectedRevenue { get; set; }
    public int PaidCount { get; set; }
    public int UnpaidCount { get; set; }
}

public class TrackOccupancyDto
{
    public int TrackId { get; set; }
    public string TrackCode { get; set; } = string.Empty;
    public string TrackName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int ActiveEnrollmentsCount { get; set; }
    public double OccupancyRatePercentage => Capacity > 0 ? Math.Round(((double)ActiveEnrollmentsCount / Capacity) * 100, 2) : 0.0;
    public bool IsFull => ActiveEnrollmentsCount >= Capacity;
}
