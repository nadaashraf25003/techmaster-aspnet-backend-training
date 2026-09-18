namespace TrainingCenter.Api.DTOs.Reports;

public class DashboardSummaryReportDto
{
    public int TotalStudents { get; set; }
    public int ActiveStudents { get; set; }
    public int TotalInstructors { get; set; }
    public int TotalTracks { get; set; }
    public int ActiveTracks { get; set; }
    public int TotalEnrollments { get; set; }
    public int ActiveEnrollments { get; set; }
    public decimal TotalExpectedRevenue { get; set; }
    public decimal TotalRealizedRevenue { get; set; }
    public decimal TotalOutstandingRevenue => Math.Max(0, TotalExpectedRevenue - TotalRealizedRevenue);
    public int CompletedPaymentsCount { get; set; }
}

public class UnpaidEnrollmentReportDto
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public int TrainingTrackId { get; set; }
    public string TrackCode { get; set; } = string.Empty;
    public string TrackTitle { get; set; } = string.Empty;
    public decimal TrackPrice { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal OutstandingBalance => Math.Max(0, TrackPrice - TotalPaid);
    public string PaymentState => TotalPaid == 0 ? "Completely Unpaid" : "Partially Paid";
}

public class TrackCapacityReportDto
{
    public int TrainingTrackId { get; set; }
    public string TrackCode { get; set; } = string.Empty;
    public string TrackTitle { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int EnrolledStudentsCount { get; set; }
    public int AvailableSeats => Math.Max(0, Capacity - EnrolledStudentsCount);
    public decimal UtilizationPercentage => Capacity > 0 ? Math.Round((EnrolledStudentsCount / (decimal)Capacity) * 100, 2) : 0;
    public bool IsFull => EnrolledStudentsCount >= Capacity;
    public string TrackStatus { get; set; } = string.Empty;
}

public class RevenueSummaryReportDto
{
    public decimal TotalRealizedRevenue { get; set; }
    public decimal TotalExpectedRevenue { get; set; }
    public decimal TotalPendingRevenue => Math.Max(0, TotalExpectedRevenue - TotalRealizedRevenue);
    public int TotalCompletedTransactions { get; set; }
    public int TotalFailedTransactions { get; set; }
    public decimal AveragePaymentAmount { get; set; }
}

public class RevenueByTrackReportDto
{
    public int TrainingTrackId { get; set; }
    public string TrackCode { get; set; } = string.Empty;
    public string TrackTitle { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int TotalEnrolledCount { get; set; }
    public decimal ExpectedRevenue => TotalEnrolledCount * UnitPrice;
    public decimal RealizedRevenue { get; set; }
    public decimal OutstandingRevenue => Math.Max(0, ExpectedRevenue - RealizedRevenue);
    public decimal CollectionRate => ExpectedRevenue > 0 ? Math.Round((RealizedRevenue / ExpectedRevenue) * 100, 2) : 0;
}

public class InstructorWorkloadReportDto
{
    public int InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int ActiveTracksAssigned { get; set; }
    public int TotalActiveStudentsSupervised { get; set; }
    public decimal TotalRevenueGenerated { get; set; }
}
