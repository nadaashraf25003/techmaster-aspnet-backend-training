namespace TrainingCenter.Api.DTOs.Reports;

public class RevenueSummaryReportDto
{
    public decimal TotalRevenue { get; set; }
    public decimal RealizedRevenue { get; set; }
    public decimal ExpectedRevenue { get; set; }
    public int PaidCount { get; set; }
    public int PendingCount { get; set; }
    public int FailedCount { get; set; }
    public int TotalTransactionsCount => PaidCount + PendingCount + FailedCount;
    public decimal AveragePaymentAmount { get; set; }
}

public class RevenueByTrackReportDto
{
    public int TrackId { get; set; }
    public string TrackCode { get; set; } = string.Empty;
    public string TrackTitle { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int EnrollmentCount { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal ExpectedRevenue { get; set; }
    public decimal CollectionRatePercentage => ExpectedRevenue > 0 ? Math.Round((TotalPaid / ExpectedRevenue) * 100, 2) : 0m;
}

public class TopTrackReportDto
{
    public int TrackId { get; set; }
    public string TrackCode { get; set; } = string.Empty;
    public string TrackTitle { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int ActiveEnrollmentCount { get; set; }
    public int TotalEnrollmentCount { get; set; }
    public int AvailableSeats { get; set; }
    public decimal UtilizationPercentage { get; set; }
    public string InstructorName { get; set; } = string.Empty;
}

public class InstructorWorkloadReportDto
{
    public int InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int NumberOfTracks { get; set; }
    public int ActiveTracksCount { get; set; }
    public int ActiveStudentsCount { get; set; }
    public int TotalStudentsSupervised { get; set; }
    public decimal TotalRevenueGenerated { get; set; }
}

public class StudentWithoutPaymentReportDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public int EnrollmentId { get; set; }
    public string EnrollmentStatus { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public int TrackId { get; set; }
    public string TrackCode { get; set; } = string.Empty;
    public string TrackTitle { get; set; } = string.Empty;
    public decimal TrackPrice { get; set; }
    public decimal TotalPaid { get; set; } = 0.00m;
    public decimal OutstandingBalance => TrackPrice - TotalPaid;
}

public class DashboardSummaryReportDto
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

public class TrackCapacityReportDto
{
    public int TrackId { get; set; }
    public string TrackCode { get; set; } = string.Empty;
    public string TrackTitle { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int EnrolledCount { get; set; }
    public int AvailableSeats => Math.Max(0, Capacity - EnrolledCount);
    public string Status { get; set; } = string.Empty;
}
