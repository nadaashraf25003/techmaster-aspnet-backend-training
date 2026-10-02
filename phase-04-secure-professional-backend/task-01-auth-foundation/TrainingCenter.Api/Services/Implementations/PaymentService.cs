using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common;
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
        if (!filters.IsDateRangeValid)
        {
            throw new BadRequestException("Invalid date range: 'from' date must be less than or equal to 'to' date.");
        }

        var query = _context.Payments
            .AsNoTracking()
            .Include(p => p.Enrollment)
                .ThenInclude(e => e!.Student)
            .Include(p => p.Enrollment)
                .ThenInclude(e => e!.TrainingTrack)
            .AsQueryable();

        if (filters.From.HasValue)
        {
            var fromDate = filters.From.Value.Date;
            query = query.Where(p => p.PaymentDate >= fromDate);
        }

        if (filters.To.HasValue)
        {
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

    /// <summary>
    /// Business Rules Enforced:
    /// - Rule P1: Payment amount must be strictly greater than 0.
    /// - Rule P6: Cannot add payment to cancelled enrollment.
    /// - Rule P2: Payment cannot exceed remaining unpaid balance (Overpayment rejected with 400).
    /// - Rule P5 and E3: Failed payments do NOT activate enrollment. Valid completed payments activate Pending enrollment.
    /// </summary>
    public async Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
    {
        // Rule P1: Amount must be positive
        if (request.Amount <= 0)
        {
            throw new BadRequestException("Payment amount must be greater than 0.");
        }

        var enrollment = await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.TrainingTrack)
            .Include(e => e.Payments)
            .FirstOrDefaultAsync(e => e.EnrollmentId == request.EnrollmentId);

        if (enrollment == null)
        {
            throw new BadRequestException($"Enrollment with ID {request.EnrollmentId} was not found.");
        }

        // Rule P6: Cannot add payment for cancelled enrollment
        if (enrollment.Status == EnrollmentStatus.Cancelled)
        {
            throw new BadRequestException("Cannot add payment for a cancelled enrollment.");
        }

        // Rule P2: Overpayment prevention
        var trackPrice = enrollment.TrainingTrack?.Price ?? 0.00m;
        var totalAlreadyPaid = enrollment.Payments
            .Where(p => p.PaymentStatus == PaymentStatus.Completed)
            .Sum(p => p.Amount);

        var remainingBalance = trackPrice - totalAlreadyPaid;

        if (request.Amount > remainingBalance)
        {
            throw new BadRequestException(
                $"Payment amount of {request.Amount:N2} EGP exceeds remaining outstanding balance of {remainingBalance:N2} EGP.",
                new List<string> { $"Maximum allowable payment for this enrollment is {remainingBalance:N2} EGP." });
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
            PaymentStatus = request.PaymentStatus,
            ReferenceNumber = refNum,
            Notes = request.Notes
        };

        await _context.Payments.AddAsync(payment);

        // Rule E3 & P5: Automatic activation on successful completed payment
        if (request.PaymentStatus == PaymentStatus.Completed && enrollment.Status == EnrollmentStatus.Pending)
        {
            enrollment.Status = EnrollmentStatus.Active;
        }

        await _context.SaveChangesAsync();

        return await GetPaymentByIdAsync(payment.PaymentId);
    }

    public async Task<PaymentResponse> UpdatePaymentStatusAsync(int id, UpdatePaymentStatusRequest request)
    {
        var payment = await _context.Payments
            .Include(p => p.Enrollment)
            .FirstOrDefaultAsync(p => p.PaymentId == id);

        if (payment == null)
        {
            throw new NotFoundException($"Payment with ID {id} was not found.");
        }

        if (!Enum.IsDefined(typeof(PaymentStatus), request.Status))
        {
            throw new BadRequestException($"Invalid payment status value '{request.Status}'.");
        }

        payment.PaymentStatus = request.Status;
        if (!string.IsNullOrWhiteSpace(request.Notes))
        {
            payment.Notes = request.Notes;
        }

        // Rule E3: If transitioned to Completed, activate pending enrollment
        if (request.Status == PaymentStatus.Completed && payment.Enrollment != null && payment.Enrollment.Status == EnrollmentStatus.Pending)
        {
            payment.Enrollment.Status = EnrollmentStatus.Active;
        }

        await _context.SaveChangesAsync();
        return await GetPaymentByIdAsync(id);
    }
}
