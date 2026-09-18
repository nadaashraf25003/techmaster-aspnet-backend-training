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

    public async Task<DashboardSummaryReportDto> GetDashboardSummaryAsync()
    {
        var totalStudents = await _context.Students.CountAsync();
        var activeStudents = await _context.Students.CountAsync(s => s.IsActive);
        var totalInstructors = await _context.Instructors.CountAsync();
        var totalTracks = await _context.TrainingTracks.CountAsync();
        var activeTracks = await _context.TrainingTracks.CountAsync(t => t.Status == TrackStatus.InProgress || t.Status == TrackStatus.Upcoming);
        var totalEnrollments = await _context.Enrollments.CountAsync();
        var activeEnrollments = await _context.Enrollments.CountAsync(e => e.Status == EnrollmentStatus.Active);

        var totalRealizedRevenue = await _context.Payments
            .Where(p => p.PaymentStatus == PaymentStatus.Completed)
            .SumAsync(p => (decimal?)p.Amount) ?? 0m;

        var totalExpectedRevenue = await _context.Enrollments
            .Include(e => e.TrainingTrack)
            .SumAsync(e => (decimal?)(e.TrainingTrack != null ? e.TrainingTrack.Price : 0m)) ?? 0m;

        var completedPaymentsCount = await _context.Payments
            .CountAsync(p => p.PaymentStatus == PaymentStatus.Completed);

        return new DashboardSummaryReportDto
        {
            TotalStudents = totalStudents,
            ActiveStudents = activeStudents,
            TotalInstructors = totalInstructors,
            TotalTracks = totalTracks,
            ActiveTracks = activeTracks,
            TotalEnrollments = totalEnrollments,
            ActiveEnrollments = activeEnrollments,
            TotalExpectedRevenue = totalExpectedRevenue,
            TotalRealizedRevenue = totalRealizedRevenue,
            CompletedPaymentsCount = completedPaymentsCount
        };
    }

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
                TrainingTrackId = e.TrainingTrackId,
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
                TrainingTrackId = x.TrainingTrackId,
                TrackCode = x.TrackCode,
                TrackTitle = x.TrackTitle,
                TrackPrice = x.TrackPrice,
                TotalPaid = x.TotalPaid
            })
            .ToListAsync();
    }

    public async Task<List<TrackCapacityReportDto>> GetTrackCapacityReportAsync()
    {
        return await _context.TrainingTracks
            .AsNoTracking()
            .Select(t => new TrackCapacityReportDto
            {
                TrainingTrackId = t.TrainingTrackId,
                TrackCode = t.Code,
                TrackTitle = t.Title,
                Capacity = t.Capacity,
                EnrolledStudentsCount = t.Enrollments.Count(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Pending),
                TrackStatus = t.Status.ToString()
            })
            .OrderByDescending(t => t.EnrolledStudentsCount)
            .ToListAsync();
    }

    public async Task<RevenueSummaryReportDto> GetRevenueSummaryAsync()
    {
        var realizedRevenue = await _context.Payments
            .Where(p => p.PaymentStatus == PaymentStatus.Completed)
            .SumAsync(p => (decimal?)p.Amount) ?? 0m;

        var expectedRevenue = await _context.Enrollments
            .Include(e => e.TrainingTrack)
            .SumAsync(e => (decimal?)(e.TrainingTrack != null ? e.TrainingTrack.Price : 0m)) ?? 0m;

        var completedCount = await _context.Payments.CountAsync(p => p.PaymentStatus == PaymentStatus.Completed);
        var failedCount = await _context.Payments.CountAsync(p => p.PaymentStatus == PaymentStatus.Failed);

        var avgAmount = completedCount > 0 ? (realizedRevenue / completedCount) : 0m;

        return new RevenueSummaryReportDto
        {
            TotalRealizedRevenue = realizedRevenue,
            TotalExpectedRevenue = expectedRevenue,
            TotalCompletedTransactions = completedCount,
            TotalFailedTransactions = failedCount,
            AveragePaymentAmount = Math.Round(avgAmount, 2)
        };
    }

    public async Task<List<RevenueByTrackReportDto>> GetRevenueByTrackReportAsync()
    {
        return await _context.TrainingTracks
            .AsNoTracking()
            .Select(t => new RevenueByTrackReportDto
            {
                TrainingTrackId = t.TrainingTrackId,
                TrackCode = t.Code,
                TrackTitle = t.Title,
                UnitPrice = t.Price,
                TotalEnrolledCount = t.Enrollments.Count,
                RealizedRevenue = t.Enrollments
                    .SelectMany(e => e.Payments)
                    .Where(p => p.PaymentStatus == PaymentStatus.Completed)
                    .Sum(p => (decimal?)p.Amount) ?? 0m
            })
            .OrderByDescending(r => r.RealizedRevenue)
            .ToListAsync();
    }

    public async Task<List<InstructorWorkloadReportDto>> GetInstructorWorkloadReportAsync()
    {
        return await _context.Instructors
            .AsNoTracking()
            .Where(i => i.IsActive)
            .Select(i => new InstructorWorkloadReportDto
            {
                InstructorId = i.InstructorId,
                InstructorName = i.FullName,
                Specialization = i.Specialization,
                ActiveTracksAssigned = i.TrainingTracks.Count(t => t.Status == TrackStatus.InProgress || t.Status == TrackStatus.Upcoming),
                TotalActiveStudentsSupervised = i.TrainingTracks
                    .SelectMany(t => t.Enrollments)
                    .Count(e => e.Status == EnrollmentStatus.Active),
                TotalRevenueGenerated = i.TrainingTracks
                    .SelectMany(t => t.Enrollments)
                    .SelectMany(e => e.Payments)
                    .Where(p => p.PaymentStatus == PaymentStatus.Completed)
                    .Sum(p => (decimal?)p.Amount) ?? 0m
            })
            .OrderByDescending(i => i.TotalActiveStudentsSupervised)
            .ThenByDescending(i => i.ActiveTracksAssigned)
            .ToListAsync();
    }
}
