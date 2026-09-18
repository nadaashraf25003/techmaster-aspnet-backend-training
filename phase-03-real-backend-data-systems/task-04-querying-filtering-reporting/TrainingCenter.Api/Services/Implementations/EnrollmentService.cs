using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common;
using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.Data;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Enrollments;
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

    /// <summary>
    /// Query 19: Advanced Enrollment Filter.
    /// Combines multiple query parameters safely using dynamic conditional IQueryable composition.
    /// Only applies filter when parameter has value.
    /// </summary>
    public async Task<PagedResult<EnrollmentListItemResponse>> GetEnrollmentsAsync(EnrollmentFilterParams filters)
    {
        var query = _context.Enrollments
            .AsNoTracking()
            .Include(e => e.Student)
            .Include(e => e.TrainingTrack)
            .Include(e => e.Payments)
            .AsQueryable();

        // 1. Conditional Filter: Track ID
        if (filters.TrackId.HasValue)
        {
            query = query.Where(e => e.TrainingTrackId == filters.TrackId.Value);
        }

        // 2. Conditional Filter: Student ID
        if (filters.StudentId.HasValue)
        {
            query = query.Where(e => e.StudentId == filters.StudentId.Value);
        }

        // 3. Conditional Filter: Enrollment Status (e.g., Active, Pending, Completed, Cancelled)
        if (filters.Status.HasValue)
        {
            query = query.Where(e => e.Status == filters.Status.Value);
        }

        // 4. Conditional Filter: Payment Status (e.g., Paid, Completed, Pending, Failed, Unpaid)
        if (!string.IsNullOrWhiteSpace(filters.PaymentStatus))
        {
            var normalized = filters.PaymentStatus.Trim().ToLowerInvariant();
            if (normalized is "paid" or "completed")
            {
                query = query.Where(e => e.Payments.Any(p => p.PaymentStatus == PaymentStatus.Completed));
            }
            else if (normalized == "pending")
            {
                query = query.Where(e => e.Payments.Any(p => p.PaymentStatus == PaymentStatus.Pending));
            }
            else if (normalized == "failed")
            {
                query = query.Where(e => e.Payments.Any(p => p.PaymentStatus == PaymentStatus.Failed));
            }
            else if (normalized == "unpaid")
            {
                query = query.Where(e => !e.Payments.Any(p => p.PaymentStatus == PaymentStatus.Completed));
            }
            else if (Enum.TryParse<PaymentStatus>(filters.PaymentStatus, true, out var parsedEnum))
            {
                query = query.Where(e => e.Payments.Any(p => p.PaymentStatus == parsedEnum));
            }
        }

        // 5. Conditional Filter: Date Range (Enrollment Date)
        if (filters.From.HasValue)
        {
            var fromDate = filters.From.Value.Date;
            query = query.Where(e => e.EnrollmentDate >= fromDate);
        }

        if (filters.To.HasValue)
        {
            var toDate = filters.To.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(e => e.EnrollmentDate <= toDate);
        }

        // 6. Conditional Filter: Search Keyword
        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var search = filters.Search.Trim().ToLower();
            query = query.Where(e => (e.Student != null && (e.Student.FullName.ToLower().Contains(search) || e.Student.Email.ToLower().Contains(search))) ||
                                     (e.TrainingTrack != null && (e.TrainingTrack.Title.ToLower().Contains(search) || e.TrainingTrack.Code.ToLower().Contains(search))));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(e => e.EnrollmentDate)
            .Skip((filters.PageNumber - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .Select(e => new EnrollmentListItemResponse
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                StudentName = e.Student != null ? e.Student.FullName : string.Empty,
                StudentEmail = e.Student != null ? e.Student.Email : string.Empty,
                TrainingTrackId = e.TrainingTrackId,
                TrackCode = e.TrainingTrack != null ? e.TrainingTrack.Code : string.Empty,
                TrackTitle = e.TrainingTrack != null ? e.TrainingTrack.Title : string.Empty,
                TrackPrice = e.TrainingTrack != null ? e.TrainingTrack.Price : 0.00m,
                EnrollmentDate = e.EnrollmentDate,
                Status = e.Status,
                ProgressPercentage = e.ProgressPercentage,
                FinalResult = e.FinalResult,
                TotalPaid = e.Payments.Where(p => p.PaymentStatus == PaymentStatus.Completed).Sum(p => (decimal?)p.Amount) ?? 0.00m
            })
            .ToListAsync();

        return new PagedResult<EnrollmentListItemResponse>(items, totalCount, filters.PageNumber, filters.PageSize);
    }

    public async Task<EnrollmentDetailsResponse> GetEnrollmentByIdAsync(int id)
    {
        var enrollment = await _context.Enrollments
            .AsNoTracking()
            .Include(e => e.Student)
            .Include(e => e.TrainingTrack)
                .ThenInclude(t => t!.PrimaryInstructor)
            .Include(e => e.Payments)
            .FirstOrDefaultAsync(e => e.EnrollmentId == id);

        if (enrollment == null)
        {
            throw new NotFoundException($"Enrollment with ID {id} was not found.");
        }

        var trackPrice = enrollment.TrainingTrack?.Price ?? 0m;
        var totalPaid = enrollment.Payments.Where(p => p.PaymentStatus == PaymentStatus.Completed).Sum(p => p.Amount);

        return new EnrollmentDetailsResponse
        {
            EnrollmentId = enrollment.EnrollmentId,
            StudentId = enrollment.StudentId,
            StudentName = enrollment.Student?.FullName ?? string.Empty,
            StudentEmail = enrollment.Student?.Email ?? string.Empty,
            StudentPhone = enrollment.Student?.PhoneNumber,
            TrainingTrackId = enrollment.TrainingTrackId,
            TrackCode = enrollment.TrainingTrack?.Code ?? string.Empty,
            TrackTitle = enrollment.TrainingTrack?.Title ?? string.Empty,
            TrackPrice = trackPrice,
            TrackStartDate = enrollment.TrainingTrack?.StartDate ?? DateTime.MinValue,
            TrackEndDate = enrollment.TrainingTrack?.EndDate ?? DateTime.MinValue,
            InstructorName = enrollment.TrainingTrack?.PrimaryInstructor?.FullName,
            EnrollmentDate = enrollment.EnrollmentDate,
            Status = enrollment.Status,
            ProgressPercentage = enrollment.ProgressPercentage,
            FinalResult = enrollment.FinalResult,
            Notes = enrollment.Notes,
            TotalPaid = totalPaid,
            Payments = enrollment.Payments.Select(p => new EnrollmentPaymentSummaryResponse
            {
                PaymentId = p.PaymentId,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                PaymentStatus = p.PaymentStatus,
                PaymentDate = p.PaymentDate,
                ReferenceNumber = p.ReferenceNumber
            }).OrderByDescending(p => p.PaymentDate).ToList()
        };
    }

    public async Task<EnrollmentDetailsResponse> CreateEnrollmentAsync(CreateEnrollmentRequest request)
    {
        var studentExists = await _context.Students.AnyAsync(s => s.StudentId == request.StudentId);
        if (!studentExists)
        {
            throw new NotFoundException($"Student with ID {request.StudentId} was not found.");
        }

        var track = await _context.TrainingTracks
            .Include(t => t.Enrollments)
            .FirstOrDefaultAsync(t => t.TrainingTrackId == request.TrainingTrackId);

        if (track == null)
        {
            throw new NotFoundException($"Training Track with ID {request.TrainingTrackId} was not found.");
        }

        var existingEnrollment = await _context.Enrollments
            .AnyAsync(e => e.StudentId == request.StudentId && e.TrainingTrackId == request.TrainingTrackId);

        if (existingEnrollment)
        {
            throw new ConflictException($"Student {request.StudentId} is already enrolled in track {request.TrainingTrackId}.");
        }

        var activeCount = track.Enrollments.Count(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Pending);
        if (activeCount >= track.Capacity)
        {
            throw new BadRequestException($"Track '{track.Title}' has reached its maximum capacity of {track.Capacity} students.");
        }

        var enrollment = new Enrollment
        {
            StudentId = request.StudentId,
            TrainingTrackId = request.TrainingTrackId,
            EnrollmentDate = DateTime.UtcNow,
            Status = request.Status,
            Notes = request.Notes
        };

        await _context.Enrollments.AddAsync(enrollment);
        await _context.SaveChangesAsync();

        return await GetEnrollmentByIdAsync(enrollment.EnrollmentId);
    }

    public async Task<EnrollmentDetailsResponse> UpdateEnrollmentStatusAsync(int id, UpdateEnrollmentStatusRequest request)
    {
        var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.EnrollmentId == id);
        if (enrollment == null)
        {
            throw new NotFoundException($"Enrollment with ID {id} was not found.");
        }

        enrollment.Status = request.Status;
        if (request.ProgressPercentage.HasValue)
        {
            enrollment.ProgressPercentage = request.ProgressPercentage.Value;
        }

        if (request.FinalResult != null)
        {
            enrollment.FinalResult = request.FinalResult;
        }

        if (request.Notes != null)
        {
            enrollment.Notes = request.Notes;
        }

        await _context.SaveChangesAsync();
        return await GetEnrollmentByIdAsync(id);
    }

    public async Task DeleteEnrollmentAsync(int id)
    {
        var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.EnrollmentId == id);
        if (enrollment == null)
        {
            throw new NotFoundException($"Enrollment with ID {id} was not found.");
        }

        enrollment.IsDeleted = true;
        await _context.SaveChangesAsync();
    }
}
