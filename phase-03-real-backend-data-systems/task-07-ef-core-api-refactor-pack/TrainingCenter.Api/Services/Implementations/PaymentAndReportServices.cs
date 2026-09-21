using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common;
using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.Data;
using TrainingCenter.Api.DTOs.Payments;
using TrainingCenter.Api.DTOs.Reports;
using TrainingCenter.Api.DTOs.Tracks;
using TrainingCenter.Api.Entities;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Services.Implementations;

public class PaymentService : IPaymentService
{
    private readonly TrainingCenterDbContext _context;

    public PaymentService(TrainingCenterDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentResponseDto> ProcessPaymentAsync(ProcessPaymentRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request.Amount <= 0)
        {
            throw new ValidationException("Payment amount must be greater than zero.");
        }

        var enrollment = await _context.Enrollments
            .Include(e => e.TrainingTrack)
            .Include(e => e.Payments)
            .FirstOrDefaultAsync(e => e.Id == request.EnrollmentId, cancellationToken);

        if (enrollment == null)
        {
            throw new NotFoundException($"Enrollment with ID {request.EnrollmentId} was not found.");
        }

        var trackPrice = enrollment.TrainingTrack?.Price ?? 0m;
        var existingPaid = enrollment.Payments
            .Where(p => p.Status == PaymentStatus.Paid && !p.IsDeleted)
            .Sum(p => p.Amount);

        var remainingBalance = trackPrice - existingPaid;

        if (request.Amount > remainingBalance)
        {
            throw new BusinessRuleException($"Payment amount of {request.Amount:N2} EGP exceeds remaining balance of {remainingBalance:N2} EGP (Track Price: {trackPrice:N2} EGP, Paid: {existingPaid:N2} EGP).");
        }

        var payment = new Payment
        {
            EnrollmentId = request.EnrollmentId,
            Amount = request.Amount,
            PaymentDate = DateTime.UtcNow,
            PaymentMethod = request.PaymentMethod,
            Status = PaymentStatus.Paid,
            ReferenceNumber = $"PAY-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}",
            Notes = request.Notes,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _context.Payments.AddAsync(payment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new PaymentResponseDto
        {
            Id = payment.Id,
            EnrollmentId = payment.EnrollmentId,
            Amount = payment.Amount,
            PaymentDate = payment.PaymentDate,
            PaymentMethod = payment.PaymentMethod,
            Status = payment.Status,
            ReferenceNumber = payment.ReferenceNumber,
            Notes = payment.Notes,
            CreatedAtUtc = payment.CreatedAtUtc
        };
    }

    public async Task<IReadOnlyList<PaymentResponseDto>> GetPaymentsByEnrollmentIdAsync(int enrollmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .AsNoTracking()
            .Where(p => p.EnrollmentId == enrollmentId)
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => new PaymentResponseDto
            {
                Id = p.Id,
                EnrollmentId = p.EnrollmentId,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                PaymentMethod = p.PaymentMethod,
                Status = p.Status,
                ReferenceNumber = p.ReferenceNumber,
                Notes = p.Notes,
                CreatedAtUtc = p.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);
    }
}

public class ReportService : IReportService
{
    private readonly TrainingCenterDbContext _context;

    public ReportService(TrainingCenterDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
    {
        var studentsCount = await _context.Students.CountAsync(cancellationToken);
        var activeStudentsCount = await _context.Students.CountAsync(s => s.IsActive, cancellationToken);
        var instructorsCount = await _context.Instructors.CountAsync(cancellationToken);
        var tracksCount = await _context.TrainingTracks.CountAsync(cancellationToken);
        var activeTracksCount = await _context.TrainingTracks.CountAsync(t => t.Status == TrackStatus.Active, cancellationToken);
        var totalEnrollmentsCount = await _context.Enrollments.CountAsync(cancellationToken);
        var activeEnrollments = await _context.Enrollments.CountAsync(e => e.Status == EnrollmentStatus.Active, cancellationToken);

        var realizedRevenue = await _context.Payments
            .Where(p => p.Status == PaymentStatus.Paid)
            .SumAsync(p => (decimal?)p.Amount, cancellationToken) ?? 0m;

        var expectedRevenue = await _context.Enrollments
            .Where(e => e.Status == EnrollmentStatus.Active)
            .SumAsync(e => (decimal?)(e.TrainingTrack != null ? e.TrainingTrack.Price : 0m), cancellationToken) ?? 0m;

        var paidCount = await _context.Payments.CountAsync(p => p.Status == PaymentStatus.Paid, cancellationToken);

        return new DashboardSummaryDto
        {
            StudentsCount = studentsCount,
            ActiveStudentsCount = activeStudentsCount,
            InstructorsCount = instructorsCount,
            TracksCount = tracksCount,
            ActiveTracksCount = activeTracksCount,
            TotalEnrollmentsCount = totalEnrollmentsCount,
            ActiveEnrollments = activeEnrollments,
            Revenue = realizedRevenue,
            RealizedRevenue = realizedRevenue,
            ExpectedRevenue = expectedRevenue,
            PaidCount = paidCount,
            UnpaidCount = Math.Max(0, totalEnrollmentsCount - paidCount)
        };
    }

    public async Task<IReadOnlyList<TrackOccupancyDto>> GetTrackOccupancyAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TrainingTracks
            .AsNoTracking()
            .Select(t => new TrackOccupancyDto
            {
                TrackId = t.Id,
                TrackCode = t.Code,
                TrackName = t.Name,
                Capacity = t.Capacity,
                ActiveEnrollmentsCount = t.Enrollments.Count(e => e.Status == EnrollmentStatus.Active && !e.IsDeleted)
            })
            .ToListAsync(cancellationToken);
    }
}
