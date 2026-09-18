using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common;
using TrainingCenter.Api.Data;
using TrainingCenter.Api.DTOs.Reports;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Services.Implementations;

public class ReportService : IReportService
{
    private readonly TrainingCenterDbContext _context;

    public ReportService(TrainingCenterDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Query 14: Revenue Summary.
    /// Returns total realized revenue, expected revenue, paid count, pending count, failed count, and average payment amount.
    /// Uses decimal precision and avoids integer money values.
    /// </summary>
    public async Task<RevenueSummaryReportDto> GetRevenueSummaryAsync()
    {
        var realizedRevenue = await _context.Payments
            .Where(p => p.PaymentStatus == PaymentStatus.Completed)
            .SumAsync(p => (decimal?)p.Amount) ?? 0.00m;

        var expectedRevenue = await _context.Enrollments
            .Where(e => e.Status != EnrollmentStatus.Cancelled)
            .Include(e => e.TrainingTrack)
            .SumAsync(e => (decimal?)(e.TrainingTrack != null ? e.TrainingTrack.Price : 0m)) ?? 0.00m;

        var paidCount = await _context.Payments.CountAsync(p => p.PaymentStatus == PaymentStatus.Completed);
        var pendingCount = await _context.Payments.CountAsync(p => p.PaymentStatus == PaymentStatus.Pending);
        var failedCount = await _context.Payments.CountAsync(p => p.PaymentStatus == PaymentStatus.Failed);

        var avgPayment = paidCount > 0 ? (realizedRevenue / paidCount) : 0.00m;

        return new RevenueSummaryReportDto
        {
            TotalRevenue = realizedRevenue,
            RealizedRevenue = realizedRevenue,
            ExpectedRevenue = expectedRevenue,
            PaidCount = paidCount,
            PendingCount = pendingCount,
            FailedCount = failedCount,
            AveragePaymentAmount = Math.Round(avgPayment, 2)
        };
    }

    /// <summary>
    /// Query 15: Revenue Per Track.
    /// Groups paid payments by track and calculates total paid amount, enrollment count, and unit price.
    /// </summary>
    public async Task<List<RevenueByTrackReportDto>> GetRevenueByTrackReportAsync()
    {
        return await _context.TrainingTracks
            .AsNoTracking()
            .Select(t => new RevenueByTrackReportDto
            {
                TrackId = t.TrainingTrackId,
                TrackCode = t.Code,
                TrackTitle = t.Title,
                UnitPrice = t.Price,
                EnrollmentCount = t.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled),
                TotalPaid = t.Enrollments
                    .SelectMany(e => e.Payments)
                    .Where(p => p.PaymentStatus == PaymentStatus.Completed)
                    .Sum(p => (decimal?)p.Amount) ?? 0.00m,
                ExpectedRevenue = t.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled) * t.Price
            })
            .OrderByDescending(r => r.TotalPaid)
            .ToListAsync();
    }

    /// <summary>
    /// Query 16: Top Tracks By Enrollment.
    /// Returns tracks ordered by active enrollment count (top 5 by default).
    /// </summary>
    public async Task<List<TopTrackReportDto>> GetTopTracksByEnrollmentAsync(int count = 5)
    {
        if (count <= 0) count = 5;

        return await _context.TrainingTracks
            .AsNoTracking()
            .Select(t => new TopTrackReportDto
            {
                TrackId = t.TrainingTrackId,
                TrackCode = t.Code,
                TrackTitle = t.Title,
                Capacity = t.Capacity,
                ActiveEnrollmentCount = t.Enrollments.Count(e => e.Status == EnrollmentStatus.Active),
                TotalEnrollmentCount = t.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled),
                AvailableSeats = Math.Max(0, t.Capacity - t.Enrollments.Count(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Pending)),
                UtilizationPercentage = t.Capacity > 0
                    ? Math.Round(((decimal)t.Enrollments.Count(e => e.Status == EnrollmentStatus.Active) / t.Capacity) * 100, 2)
                    : 0.00m,
                InstructorName = t.PrimaryInstructor != null ? t.PrimaryInstructor.FullName : "Unassigned"
            })
            .OrderByDescending(t => t.ActiveEnrollmentCount)
            .ThenByDescending(t => t.TotalEnrollmentCount)
            .Take(count)
            .ToListAsync();
    }

    /// <summary>
    /// Query 17: Instructor Workload.
    /// Returns each active instructor with assigned tracks, active supervised students, and total revenue.
    /// </summary>
    public async Task<List<InstructorWorkloadReportDto>> GetInstructorWorkloadReportAsync()
    {
        return await _context.Instructors
            .AsNoTracking()
            .Where(i => i.IsActive)
            .Select(i => new InstructorWorkloadReportDto
            {
                InstructorId = i.InstructorId,
                InstructorName = i.FullName,
                Email = i.Email,
                Specialization = i.Specialization,
                NumberOfTracks = i.TrainingTracks.Count,
                ActiveTracksCount = i.TrainingTracks.Count(t => t.Status == TrackStatus.InProgress || t.Status == TrackStatus.Upcoming),
                ActiveStudentsCount = i.TrainingTracks
                    .SelectMany(t => t.Enrollments)
                    .Count(e => e.Status == EnrollmentStatus.Active),
                TotalStudentsSupervised = i.TrainingTracks
                    .SelectMany(t => t.Enrollments)
                    .Count(e => e.Status != EnrollmentStatus.Cancelled),
                TotalRevenueGenerated = i.TrainingTracks
                    .SelectMany(t => t.Enrollments)
                    .SelectMany(e => e.Payments)
                    .Where(p => p.PaymentStatus == PaymentStatus.Completed)
                    .Sum(p => (decimal?)p.Amount) ?? 0.00m
            })
            .OrderByDescending(i => i.ActiveStudentsCount)
            .ThenByDescending(i => i.ActiveTracksCount)
            .ToListAsync();
    }

    /// <summary>
    /// Query 18: Students Without Payments.
    /// Returns students with Active or Pending enrollments who have zero completed payments.
    /// Must strictly NOT include cancelled enrollments.
    /// </summary>
    public async Task<List<StudentWithoutPaymentReportDto>> GetStudentsWithoutPaymentsAsync()
    {
        return await _context.Enrollments
            .AsNoTracking()
            .Where(e => (e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Pending)
                        && !e.Payments.Any(p => p.PaymentStatus == PaymentStatus.Completed))
            .Include(e => e.Student)
            .Include(e => e.TrainingTrack)
            .Select(e => new StudentWithoutPaymentReportDto
            {
                StudentId = e.StudentId,
                StudentName = e.Student != null ? e.Student.FullName : string.Empty,
                Email = e.Student != null ? e.Student.Email : string.Empty,
                PhoneNumber = e.Student != null ? e.Student.PhoneNumber : null,
                EnrollmentId = e.EnrollmentId,
                EnrollmentStatus = e.Status.ToString(),
                EnrollmentDate = e.EnrollmentDate,
                TrackId = e.TrainingTrackId,
                TrackCode = e.TrainingTrack != null ? e.TrainingTrack.Code : string.Empty,
                TrackTitle = e.TrainingTrack != null ? e.TrainingTrack.Title : string.Empty,
                TrackPrice = e.TrainingTrack != null ? e.TrainingTrack.Price : 0.00m,
                TotalPaid = 0.00m
            })
            .OrderBy(s => s.StudentName)
            .ToListAsync();
    }

    /// <summary>
    /// Query 20: Dashboard Summary.
    /// Aggregates high-level system numbers in one response: studentsCount, tracksCount, activeEnrollments, revenue, unpaidCount.
    /// </summary>
    public async Task<DashboardSummaryReportDto> GetDashboardSummaryAsync()
    {
        var totalStudents = await _context.Students.CountAsync();
        var activeStudents = await _context.Students.CountAsync(s => s.IsActive);
        var totalInstructors = await _context.Instructors.CountAsync();
        var totalTracks = await _context.TrainingTracks.CountAsync();
        var activeTracks = await _context.TrainingTracks.CountAsync(t => t.Status == TrackStatus.InProgress || t.Status == TrackStatus.Upcoming);
        var totalEnrollments = await _context.Enrollments.CountAsync();
        var activeEnrollments = await _context.Enrollments.CountAsync(e => e.Status == EnrollmentStatus.Active);

        var realizedRevenue = await _context.Payments
            .Where(p => p.PaymentStatus == PaymentStatus.Completed)
            .SumAsync(p => (decimal?)p.Amount) ?? 0.00m;

        var expectedRevenue = await _context.Enrollments
            .Where(e => e.Status != EnrollmentStatus.Cancelled)
            .Include(e => e.TrainingTrack)
            .SumAsync(e => (decimal?)(e.TrainingTrack != null ? e.TrainingTrack.Price : 0m)) ?? 0.00m;

        var paidCount = await _context.Payments.CountAsync(p => p.PaymentStatus == PaymentStatus.Completed);

        // Calculate count of active/pending enrollments with unpaid balance
        var unpaidCount = await _context.Enrollments
            .Where(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Pending)
            .Where(e => (e.Payments.Where(p => p.PaymentStatus == PaymentStatus.Completed).Sum(p => (decimal?)p.Amount) ?? 0m) < (e.TrainingTrack != null ? e.TrainingTrack.Price : 0m))
            .CountAsync();

        return new DashboardSummaryReportDto
        {
            StudentsCount = totalStudents,
            ActiveStudentsCount = activeStudents,
            InstructorsCount = totalInstructors,
            TracksCount = totalTracks,
            ActiveTracksCount = activeTracks,
            TotalEnrollmentsCount = totalEnrollments,
            ActiveEnrollments = activeEnrollments,
            Revenue = realizedRevenue,
            RealizedRevenue = realizedRevenue,
            ExpectedRevenue = expectedRevenue,
            PaidCount = paidCount,
            UnpaidCount = unpaidCount
        };
    }

    /// <summary>
    /// Supporting report: Unpaid / partially paid enrollments list.
    /// </summary>
    public async Task<List<UnpaidEnrollmentReportDto>> GetUnpaidEnrollmentsAsync()
    {
        return await _context.Enrollments
            .AsNoTracking()
            .Where(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Pending)
            .Include(e => e.Student)
            .Include(e => e.TrainingTrack)
            .Include(e => e.Payments)
            .Select(e => new
            {
                e.EnrollmentId,
                StudentId = e.Student != null ? e.Student.StudentId : 0,
                StudentName = e.Student != null ? e.Student.FullName : string.Empty,
                StudentEmail = e.Student != null ? e.Student.Email : string.Empty,
                PhoneNumber = e.Student != null ? e.Student.PhoneNumber : null,
                TrackId = e.TrainingTrackId,
                TrackCode = e.TrainingTrack != null ? e.TrainingTrack.Code : string.Empty,
                TrackTitle = e.TrainingTrack != null ? e.TrainingTrack.Title : string.Empty,
                TrackPrice = e.TrainingTrack != null ? e.TrainingTrack.Price : 0m,
                TotalPaid = e.Payments.Where(p => p.PaymentStatus == PaymentStatus.Completed).Sum(p => (decimal?)p.Amount) ?? 0m
            })
            .Where(x => x.TotalPaid < x.TrackPrice)
            .OrderByDescending(x => x.TrackPrice - x.TotalPaid)
            .Select(x => new UnpaidEnrollmentReportDto
            {
                EnrollmentId = x.EnrollmentId,
                StudentId = x.StudentId,
                StudentName = x.StudentName,
                StudentEmail = x.StudentEmail,
                PhoneNumber = x.PhoneNumber,
                TrackId = x.TrackId,
                TrackCode = x.TrackCode,
                TrackTitle = x.TrackTitle,
                TrackPrice = x.TrackPrice,
                TotalPaid = x.TotalPaid
            })
            .ToListAsync();
    }

    /// <summary>
    /// Supporting report: Track capacity and available seats.
    /// </summary>
    public async Task<List<TrackCapacityReportDto>> GetTrackCapacityReportAsync()
    {
        return await _context.TrainingTracks
            .AsNoTracking()
            .Select(t => new TrackCapacityReportDto
            {
                TrackId = t.TrainingTrackId,
                TrackCode = t.Code,
                TrackTitle = t.Title,
                Capacity = t.Capacity,
                EnrolledCount = t.Enrollments.Count(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Pending),
                Status = t.Status.ToString()
            })
            .OrderByDescending(t => t.EnrolledCount)
            .ToListAsync();
    }
}
