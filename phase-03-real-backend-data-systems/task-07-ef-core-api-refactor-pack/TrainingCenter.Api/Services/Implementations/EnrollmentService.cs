using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common;
using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.Data;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Enrollments;
using TrainingCenter.Api.DTOs.Payments;
using TrainingCenter.Api.Entities;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Services.Implementations;

public class EnrollmentService : IEnrollmentService
{
    private readonly TrainingCenterDbContext _context;

    public EnrollmentService(TrainingCenterDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<EnrollmentResponseDto>> GetAllPagedAsync(EnrollmentQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        var query = _context.Enrollments
            .AsNoTracking()
            .AsQueryable();

        // Apply filters
        if (parameters.StudentId.HasValue)
        {
            query = query.Where(e => e.StudentId == parameters.StudentId.Value);
        }

        if (parameters.TrackId.HasValue)
        {
            query = query.Where(e => e.TrainingTrackId == parameters.TrackId.Value);
        }

        if (parameters.Status.HasValue)
        {
            query = query.Where(e => e.Status == parameters.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var term = parameters.SearchTerm.Trim().ToLower();
            query = query.Where(e =>
                (e.Student != null && e.Student.FullName.ToLower().Contains(term)) ||
                (e.TrainingTrack != null && (e.TrainingTrack.Code.ToLower().Contains(term) || e.TrainingTrack.Name.ToLower().Contains(term))));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        // Project directly to DTOs in SQL using EF Core
        var items = await query
            .OrderByDescending(e => e.EnrollmentDate)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Select(e => new EnrollmentResponseDto
            {
                Id = e.Id,
                StudentId = e.StudentId,
                StudentName = e.Student != null ? e.Student.FullName : "Unknown Student",
                StudentEmail = e.Student != null ? e.Student.Email : string.Empty,
                TrainingTrackId = e.TrainingTrackId,
                TrackCode = e.TrainingTrack != null ? e.TrainingTrack.Code : string.Empty,
                TrackName = e.TrainingTrack != null ? e.TrainingTrack.Name : string.Empty,
                TrackPrice = e.TrainingTrack != null ? e.TrainingTrack.Price : 0,
                EnrollmentDate = e.EnrollmentDate,
                Status = e.Status,
                TotalPaidAmount = e.Payments.Where(p => p.Status == PaymentStatus.Paid).Sum(p => (decimal?)p.Amount) ?? 0m
            })
            .ToListAsync(cancellationToken);

        // Post-filter for payment status if requested
        if (parameters.IsFullyPaid.HasValue)
        {
            items = items.Where(i => i.IsFullyPaid == parameters.IsFullyPaid.Value).ToList();
        }

        return new PagedResult<EnrollmentResponseDto>(items, totalCount, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<EnrollmentDetailResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var enrollment = await _context.Enrollments
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new EnrollmentDetailResponseDto
            {
                Id = e.Id,
                StudentId = e.StudentId,
                StudentName = e.Student != null ? e.Student.FullName : "Unknown",
                StudentEmail = e.Student != null ? e.Student.Email : string.Empty,
                TrainingTrackId = e.TrainingTrackId,
                TrackCode = e.TrainingTrack != null ? e.TrainingTrack.Code : string.Empty,
                TrackName = e.TrainingTrack != null ? e.TrainingTrack.Name : string.Empty,
                TrackPrice = e.TrainingTrack != null ? e.TrainingTrack.Price : 0,
                EnrollmentDate = e.EnrollmentDate,
                Status = e.Status,
                TotalPaidAmount = e.Payments.Where(p => p.Status == PaymentStatus.Paid).Sum(p => (decimal?)p.Amount) ?? 0m,
                CreatedAtUtc = e.CreatedAtUtc,
                UpdatedAtUtc = e.UpdatedAtUtc,
                Payments = e.Payments.Select(p => new PaymentResponseDto
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
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (enrollment == null)
        {
            throw new NotFoundException($"Enrollment with ID {id} was not found.");
        }

        return enrollment;
    }

    public async Task<EnrollmentResponseDto> CreateAsync(CreateEnrollmentRequestDto request, CancellationToken cancellationToken = default)
    {
        // 1. Verify Student exists and is active
        var student = await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.StudentId, cancellationToken);

        if (student == null)
        {
            throw new NotFoundException($"Student with ID {request.StudentId} was not found.");
        }

        if (!student.IsActive)
        {
            throw new BusinessRuleException($"Student '{student.FullName}' is inactive and cannot be enrolled.");
        }

        // 2. Verify Track exists, is active, and check capacity
        var track = await _context.TrainingTracks
            .Include(t => t.Enrollments)
            .FirstOrDefaultAsync(t => t.Id == request.TrainingTrackId, cancellationToken);

        if (track == null)
        {
            throw new NotFoundException($"Training Track with ID {request.TrainingTrackId} was not found.");
        }

        if (track.Status != TrackStatus.Active && track.Status != TrackStatus.Planned)
        {
            throw new BusinessRuleException($"Cannot enroll in track '{track.Name}' because its status is '{track.Status}'.");
        }

        // 3. Prevent duplicate active enrollment
        var isAlreadyEnrolled = await _context.Enrollments
            .AnyAsync(e => e.StudentId == request.StudentId &&
                           e.TrainingTrackId == request.TrainingTrackId &&
                           e.Status == EnrollmentStatus.Active, cancellationToken);

        if (isAlreadyEnrolled)
        {
            throw new ConflictException($"Student '{student.FullName}' is already actively enrolled in Track '{track.Name}'. Duplicate active enrollments are prohibited.");
        }

        // 4. Check Track Capacity
        var activeEnrollmentsCount = track.Enrollments.Count(e => e.Status == EnrollmentStatus.Active && !e.IsDeleted);
        if (activeEnrollmentsCount >= track.Capacity)
        {
            throw new BusinessRuleException($"Track '{track.Name}' has reached maximum capacity ({track.Capacity} students). Registration closed.");
        }

        // 5. Create new enrollment
        var enrollment = new Enrollment
        {
            StudentId = request.StudentId,
            TrainingTrackId = request.TrainingTrackId,
            EnrollmentDate = DateTime.UtcNow,
            Status = EnrollmentStatus.Active,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _context.Enrollments.AddAsync(enrollment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new EnrollmentResponseDto
        {
            Id = enrollment.Id,
            StudentId = student.Id,
            StudentName = student.FullName,
            StudentEmail = student.Email,
            TrainingTrackId = track.Id,
            TrackCode = track.Code,
            TrackName = track.Name,
            TrackPrice = track.Price,
            EnrollmentDate = enrollment.EnrollmentDate,
            Status = enrollment.Status,
            TotalPaidAmount = 0m
        };
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

    public async Task SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var enrollment = await _context.Enrollments.FindAsync(new object[] { id }, cancellationToken);

        if (enrollment == null)
        {
            throw new NotFoundException($"Enrollment with ID {id} was not found.");
        }

        // Soft delete preserves audit and payment trails
        enrollment.IsDeleted = true;
        enrollment.DeletedAtUtc = DateTime.UtcNow;
        enrollment.UpdatedAtUtc = DateTime.UtcNow;
        enrollment.Status = EnrollmentStatus.Cancelled;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
