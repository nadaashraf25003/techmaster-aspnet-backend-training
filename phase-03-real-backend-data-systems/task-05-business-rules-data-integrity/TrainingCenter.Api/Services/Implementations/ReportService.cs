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

    public async Task<RevenueSummaryReportDto> GetRevenueSummaryAsync()
    {
        var payments = await _context.Payments
            .AsNoTracking()
            .ToListAsync();

        var paidPayments = payments.Where(p => p.PaymentStatus == PaymentStatus.Completed).ToList();
        var totalRevenue = paidPayments.Sum(p => p.Amount);
        var paidCount = paidPayments.Count;
        var pendingCount = payments.Count(p => p.PaymentStatus == PaymentStatus.Pending);
        var failedCount = payments.Count(p => p.PaymentStatus == PaymentStatus.Failed);

        var enrollments = await _context.Enrollments
            .AsNoTracking()
            .Include(e => e.TrainingTrack)
            .Where(e => e.Status != EnrollmentStatus.Cancelled)
            .ToListAsync();

        var expectedRevenue = enrollments.Sum(e => e.TrainingTrack?.Price ?? 0m);

        return new RevenueSummaryReportDto
        {
            TotalRevenue = totalRevenue,
            RealizedRevenue = totalRevenue,
            ExpectedRevenue = expectedRevenue,
            PaidCount = paidCount,
            PendingCount = pendingCount,
            FailedCount = failedCount,
            AveragePaymentAmount = paidCount > 0 ? Math.Round(totalRevenue / paidCount, 2) : 0m
        };
    }

    public async Task<List<RevenueByTrackReportDto>> GetRevenueByTrackReportAsync()
    {
        var tracks = await _context.TrainingTracks
            .AsNoTracking()
            .Include(t => t.Enrollments)
                .ThenInclude(e => e.Payments)
            .ToListAsync();

        return tracks.Select(t =>
        {
            var validEnrollments = t.Enrollments.Where(e => e.Status != EnrollmentStatus.Cancelled).ToList();
            var totalPaid = validEnrollments
                .SelectMany(e => e.Payments)
                .Where(p => p.PaymentStatus == PaymentStatus.Completed)
                .Sum(p => p.Amount);
            var expectedRevenue = t.Price * validEnrollments.Count;

            return new RevenueByTrackReportDto
            {
                TrackId = t.TrainingTrackId,
                TrackCode = t.Code,
                TrackTitle = t.Title,
                UnitPrice = t.Price,
                EnrollmentCount = validEnrollments.Count,
                TotalPaid = totalPaid,
                ExpectedRevenue = expectedRevenue
            };
        }).OrderByDescending(r => r.TotalPaid).ToList();
    }

    public async Task<List<TopTrackReportDto>> GetTopTracksByEnrollmentAsync(int count = 5)
    {
        var tracks = await _context.TrainingTracks
            .AsNoTracking()
            .Include(t => t.PrimaryInstructor)
            .Include(t => t.Enrollments)
            .ToListAsync();

        return tracks.Select(t =>
        {
            var activeCount = t.Enrollments.Count(e => e.Status == EnrollmentStatus.Active);
            var totalCount = t.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled);
            var available = Math.Max(0, t.Capacity - activeCount);
            var utilization = t.Capacity > 0 ? Math.Round(((decimal)activeCount / t.Capacity) * 100, 2) : 0m;

            return new TopTrackReportDto
            {
                TrackId = t.TrainingTrackId,
                TrackCode = t.Code,
                TrackTitle = t.Title,
                Capacity = t.Capacity,
                ActiveEnrollmentCount = activeCount,
                TotalEnrollmentCount = totalCount,
                AvailableSeats = available,
                UtilizationPercentage = utilization,
                InstructorName = t.PrimaryInstructor != null ? t.PrimaryInstructor.FullName : "Unassigned"
            };
        })
        .OrderByDescending(t => t.ActiveEnrollmentCount)
        .ThenByDescending(t => t.UtilizationPercentage)
        .Take(count)
        .ToList();
    }

    public async Task<List<InstructorWorkloadReportDto>> GetInstructorWorkloadReportAsync()
    {
        var instructors = await _context.Instructors
            .AsNoTracking()
            .Include(i => i.TrainingTracks)
                .ThenInclude(t => t.Enrollments)
                    .ThenInclude(e => e.Payments)
            .ToListAsync();

        return instructors.Select(i =>
        {
            var tracks = i.TrainingTracks.ToList();
            var activeTracks = tracks.Count(t => t.Status == TrackStatus.InProgress || t.Status == TrackStatus.Upcoming);
            var allValidEnrollments = tracks
                .SelectMany(t => t.Enrollments)
                .Where(e => e.Status != EnrollmentStatus.Cancelled)
                .ToList();
            var activeStudents = allValidEnrollments
                .Where(e => e.Status == EnrollmentStatus.Active)
                .Select(e => e.StudentId)
                .Distinct()
                .Count();
            var totalStudents = allValidEnrollments
                .Select(e => e.StudentId)
                .Distinct()
                .Count();
            var totalRevenue = allValidEnrollments
                .SelectMany(e => e.Payments)
                .Where(p => p.PaymentStatus == PaymentStatus.Completed)
                .Sum(p => p.Amount);

            return new InstructorWorkloadReportDto
            {
                InstructorId = i.InstructorId,
                InstructorName = i.FullName,
                Email = i.Email,
                Specialization = i.Specialization,
                NumberOfTracks = tracks.Count,
                ActiveTracksCount = activeTracks,
                ActiveStudentsCount = activeStudents,
                TotalStudentsSupervised = totalStudents,
                TotalRevenueGenerated = totalRevenue
            };
        }).OrderByDescending(i => i.TotalStudentsSupervised).ToList();
    }

    public async Task<List<StudentWithoutPaymentReportDto>> GetStudentsWithoutPaymentsAsync()
    {
        var enrollments = await _context.Enrollments
            .AsNoTracking()
            .Include(e => e.Student)
            .Include(e => e.TrainingTrack)
            .Include(e => e.Payments)
            .Where(e => e.Status != EnrollmentStatus.Cancelled)
            .ToListAsync();

        var result = new List<StudentWithoutPaymentReportDto>();

        foreach (var e in enrollments)
        {
            var completedPaymentsTotal = e.Payments
                .Where(p => p.PaymentStatus == PaymentStatus.Completed)
                .Sum(p => p.Amount);

            if (completedPaymentsTotal == 0)
            {
                result.Add(new StudentWithoutPaymentReportDto
                {
                    StudentId = e.StudentId,
                    StudentName = e.Student?.FullName ?? string.Empty,
                    Email = e.Student?.Email ?? string.Empty,
                    PhoneNumber = e.Student?.PhoneNumber,
                    EnrollmentId = e.EnrollmentId,
                    EnrollmentStatus = e.Status.ToString(),
                    EnrollmentDate = e.EnrollmentDate,
                    TrackId = e.TrainingTrackId,
                    TrackCode = e.TrainingTrack?.Code ?? string.Empty,
                    TrackTitle = e.TrainingTrack?.Title ?? string.Empty,
                    TrackPrice = e.TrainingTrack?.Price ?? 0m,
                    TotalPaid = 0m
                });
            }
        }

        return result.OrderBy(r => r.StudentName).ToList();
    }

    public async Task<DashboardSummaryReportDto> GetDashboardSummaryAsync()
    {
        var studentsCount = await _context.Students.CountAsync();
        var activeStudentsCount = await _context.Students.CountAsync(s => s.IsActive);
        var instructorsCount = await _context.Instructors.CountAsync();
        var tracksCount = await _context.TrainingTracks.CountAsync();
        var activeTracksCount = await _context.TrainingTracks.CountAsync(t => t.Status == TrackStatus.InProgress || t.Status == TrackStatus.Upcoming);
        
        var totalEnrollmentsCount = await _context.Enrollments.CountAsync(e => e.Status != EnrollmentStatus.Cancelled);
        var activeEnrollments = await _context.Enrollments.CountAsync(e => e.Status == EnrollmentStatus.Active);

        var completedPayments = await _context.Payments
            .Where(p => p.PaymentStatus == PaymentStatus.Completed)
            .ToListAsync();

        var revenue = completedPayments.Sum(p => p.Amount);
        var paidCount = completedPayments.Count;

        var enrollmentsWithPayments = await _context.Enrollments
            .Include(e => e.TrainingTrack)
            .Include(e => e.Payments)
            .Where(e => e.Status != EnrollmentStatus.Cancelled)
            .ToListAsync();

        var expectedRevenue = enrollmentsWithPayments.Sum(e => e.TrainingTrack?.Price ?? 0m);
        var unpaidCount = enrollmentsWithPayments.Count(e => !e.Payments.Any(p => p.PaymentStatus == PaymentStatus.Completed));

        return new DashboardSummaryReportDto
        {
            StudentsCount = studentsCount,
            ActiveStudentsCount = activeStudentsCount,
            InstructorsCount = instructorsCount,
            TracksCount = tracksCount,
            ActiveTracksCount = activeTracksCount,
            TotalEnrollmentsCount = totalEnrollmentsCount,
            ActiveEnrollments = activeEnrollments,
            Revenue = revenue,
            RealizedRevenue = revenue,
            ExpectedRevenue = expectedRevenue,
            PaidCount = paidCount,
            UnpaidCount = unpaidCount
        };
    }

    public async Task<List<TrackCapacityReportDto>> GetTrackCapacityReportAsync()
    {
        var tracks = await _context.TrainingTracks
            .AsNoTracking()
            .Include(t => t.Enrollments)
            .ToListAsync();

        return tracks.Select(t =>
        {
            var enrolledCount = t.Enrollments.Count(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Pending);
            return new TrackCapacityReportDto
            {
                TrackId = t.TrainingTrackId,
                TrackCode = t.Code,
                TrackTitle = t.Title,
                Capacity = t.Capacity,
                EnrolledCount = enrolledCount,
                Status = t.Status.ToString()
            };
        }).OrderBy(t => t.TrackCode).ToList();
    }
}
