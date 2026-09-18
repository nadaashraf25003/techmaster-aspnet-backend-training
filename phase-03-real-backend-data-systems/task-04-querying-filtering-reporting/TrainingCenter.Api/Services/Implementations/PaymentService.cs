using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.Data;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Payments;
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

    /// <summary>
    /// Query 13: Payments By Date Range.
    /// Filters payments between from and to dates, validates from &lt;= to, supports status/method filters and pagination.
    /// </summary>
    public async Task<PagedResult<PaymentResponse>> GetPaymentsAsync(PaymentFilterParams filters)
    {
        // Query 13 Validation: Validate from <= to
        if (filters.From.HasValue && filters.To.HasValue && filters.From.Value > filters.To.Value)
        {
            throw new BadRequestException("Invalid date range: 'from' date must be less than or equal to 'to' date.", new List<string>
            {
                $"'from' date ({filters.From.Value:yyyy-MM-dd}) cannot be after 'to' date ({filters.To.Value:yyyy-MM-dd})."
            });
        }

        var query = _context.Payments
            .AsNoTracking()
            .Include(p => p.Enrollment)
                .ThenInclude(e => e!.Student)
            .Include(p => p.Enrollment)
                .ThenInclude(e => e!.TrainingTrack)
            .AsQueryable();

        // Query 13: EF Core Concept - Where PaymentDate between from/to
        if (filters.From.HasValue)
        {
            var fromDate = filters.From.Value.Date;
            query = query.Where(p => p.PaymentDate >= fromDate);
        }

        if (filters.To.HasValue)
        {
            // End of day inclusion
            var toDate = filters.To.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(p => p.PaymentDate <= toDate);
        }

        if (filters.Status.HasValue)
        {
            query = query.Where(p => p.PaymentStatus == filters.Status.Value);
        }

        if (filters.Method.HasValue)
        {
            query = query.Where(p => p.PaymentMethod == filters.Method.Value);
        }

        if (filters.EnrollmentId.HasValue)
        {
            query = query.Where(p => p.EnrollmentId == filters.EnrollmentId.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.PaymentDate)
            .Skip((filters.PageNumber - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .Select(p => new PaymentResponse
            {
                PaymentId = p.PaymentId,
                EnrollmentId = p.EnrollmentId,
                StudentName = p.Enrollment != null && p.Enrollment.Student != null ? p.Enrollment.Student.FullName : string.Empty,
                TrackTitle = p.Enrollment != null && p.Enrollment.TrainingTrack != null ? p.Enrollment.TrainingTrack.Title : string.Empty,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                PaymentDate = p.PaymentDate,
                PaymentStatus = p.PaymentStatus,
                ReferenceNumber = p.ReferenceNumber,
                Notes = p.Notes,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<PaymentResponse>(items, totalCount, filters.PageNumber, filters.PageSize);
    }

    public async Task<PaymentResponse> GetPaymentByIdAsync(int id)
    {
        var payment = await _context.Payments
            .AsNoTracking()
            .Include(p => p.Enrollment)
                .ThenInclude(e => e!.Student)
            .Include(p => p.Enrollment)
                .ThenInclude(e => e!.TrainingTrack)
            .FirstOrDefaultAsync(p => p.PaymentId == id);

        if (payment == null)
        {
            throw new NotFoundException($"Payment with ID {id} was not found.");
        }

        return new PaymentResponse
        {
            PaymentId = payment.PaymentId,
            EnrollmentId = payment.EnrollmentId,
            StudentName = payment.Enrollment?.Student?.FullName ?? string.Empty,
            TrackTitle = payment.Enrollment?.TrainingTrack?.Title ?? string.Empty,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod,
            PaymentDate = payment.PaymentDate,
            PaymentStatus = payment.PaymentStatus,
            ReferenceNumber = payment.ReferenceNumber,
            Notes = payment.Notes,
            CreatedAt = payment.CreatedAt
        };
    }

    public async Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.TrainingTrack)
            .Include(e => e.Payments)
            .FirstOrDefaultAsync(e => e.EnrollmentId == request.EnrollmentId);

        if (enrollment == null)
        {
            throw new NotFoundException($"Enrollment with ID {request.EnrollmentId} was not found.");
        }

        if (enrollment.Status == Common.EnrollmentStatus.Cancelled)
        {
            throw new BadRequestException("Cannot add payment for a cancelled enrollment.");
        }

        var refNum = string.IsNullOrWhiteSpace(request.ReferenceNumber)
            ? $"PAY-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}"
            : request.ReferenceNumber;

        var existsRef = await _context.Payments.AnyAsync(p => p.ReferenceNumber == refNum);
        if (existsRef)
        {
            throw new ConflictException($"A payment with reference number '{refNum}' already exists.");
        }

        var payment = new Payment
        {
            EnrollmentId = request.EnrollmentId,
            Amount = request.Amount,
            PaymentMethod = request.PaymentMethod,
            PaymentDate = request.PaymentDate,
            PaymentStatus = Common.PaymentStatus.Completed,
            ReferenceNumber = refNum,
            Notes = request.Notes
        };

        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();

        return await GetPaymentByIdAsync(payment.PaymentId);
    }

    public async Task<PaymentResponse> UpdatePaymentStatusAsync(int id, UpdatePaymentStatusRequest request)
    {
        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == id);
        if (payment == null)
        {
            throw new NotFoundException($"Payment with ID {id} was not found.");
        }

        payment.PaymentStatus = request.Status;
        if (!string.IsNullOrWhiteSpace(request.Notes))
        {
            payment.Notes = request.Notes;
        }

        await _context.SaveChangesAsync();
        return await GetPaymentByIdAsync(id);
    }
}
