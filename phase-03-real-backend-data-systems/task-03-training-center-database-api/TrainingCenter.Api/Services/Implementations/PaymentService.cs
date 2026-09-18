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

    public async Task<PagedResult<PaymentResponse>> GetPaymentsAsync(PaymentFilterParams filters)
    {
        var query = _context.Payments
            .AsNoTracking()
            .Include(p => p.Enrollment)
                .ThenInclude(e => e!.Student)
            .Include(p => p.Enrollment)
                .ThenInclude(e => e!.TrainingTrack)
            .AsQueryable();

        if (filters.StartDate.HasValue)
        {
            query = query.Where(p => p.PaymentDate >= filters.StartDate.Value);
        }

        if (filters.EndDate.HasValue)
        {
            query = query.Where(p => p.PaymentDate <= filters.EndDate.Value);
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
        // 1. Validate Enrollment exists
        var enrollment = await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.TrainingTrack)
            .Include(e => e.Payments)
            .FirstOrDefaultAsync(e => e.EnrollmentId == request.EnrollmentId);

        if (enrollment == null)
        {
            throw new BadRequestException($"Enrollment with ID {request.EnrollmentId} does not exist.");
        }

        // 2. Validate unique reference number
        var refExists = await _context.Payments.AnyAsync(p => p.ReferenceNumber.ToLower() == request.ReferenceNumber.Trim().ToLower());
        if (refExists)
        {
            throw new ConflictException($"A payment with reference number '{request.ReferenceNumber}' already exists.");
        }

        var payment = new Payment
        {
            EnrollmentId = request.EnrollmentId,
            Amount = request.Amount,
            PaymentMethod = request.PaymentMethod,
            PaymentDate = DateTime.UtcNow,
            PaymentStatus = request.PaymentStatus,
            ReferenceNumber = request.ReferenceNumber.Trim(),
            Notes = request.Notes?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();

        return new PaymentResponse
        {
            PaymentId = payment.PaymentId,
            EnrollmentId = payment.EnrollmentId,
            StudentName = enrollment.Student?.FullName ?? string.Empty,
            TrackTitle = enrollment.TrainingTrack?.Title ?? string.Empty,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod,
            PaymentDate = payment.PaymentDate,
            PaymentStatus = payment.PaymentStatus,
            ReferenceNumber = payment.ReferenceNumber,
            Notes = payment.Notes,
            CreatedAt = payment.CreatedAt
        };
    }

    public async Task<List<PaymentResponse>> GetPaymentsByEnrollmentIdAsync(int enrollmentId)
    {
        var enrollmentExists = await _context.Enrollments.AnyAsync(e => e.EnrollmentId == enrollmentId);
        if (!enrollmentExists)
        {
            throw new NotFoundException($"Enrollment with ID {enrollmentId} was not found.");
        }

        return await _context.Payments
            .AsNoTracking()
            .Where(p => p.EnrollmentId == enrollmentId)
            .Include(p => p.Enrollment)
                .ThenInclude(e => e!.Student)
            .Include(p => p.Enrollment)
                .ThenInclude(e => e!.TrainingTrack)
            .OrderByDescending(p => p.PaymentDate)
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
    }

    public async Task<PaymentResponse> UpdatePaymentStatusAsync(int id, UpdatePaymentStatusRequest request)
    {
        var payment = await _context.Payments
            .Include(p => p.Enrollment)
                .ThenInclude(e => e!.Student)
            .Include(p => p.Enrollment)
                .ThenInclude(e => e!.TrainingTrack)
            .FirstOrDefaultAsync(p => p.PaymentId == id);

        if (payment == null)
        {
            throw new NotFoundException($"Payment with ID {id} was not found.");
        }

        payment.PaymentStatus = request.Status;
        if (!string.IsNullOrWhiteSpace(request.Notes))
        {
            payment.Notes = request.Notes.Trim();
        }

        await _context.SaveChangesAsync();

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
}
