using Microsoft.EntityFrameworkCore;
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

    public async Task<PagedResult<EnrollmentListItemResponse>> GetEnrollmentsAsync(EnrollmentFilterParams filters)
    {
        var query = _context.Enrollments
            .AsNoTracking()
            .Include(e => e.Student)
            .Include(e => e.TrainingTrack)
            .Include(e => e.Payments)
            .AsQueryable();

        if (filters.Status.HasValue)
        {
            query = query.Where(e => e.Status == filters.Status.Value);
        }

        if (filters.TrackId.HasValue)
        {
            query = query.Where(e => e.TrainingTrackId == filters.TrackId.Value);
        }

        if (filters.StudentId.HasValue)
        {
            query = query.Where(e => e.StudentId == filters.StudentId.Value);
        }

        if (filters.PaymentStatus.HasValue)
        {
            query = query.Where(e => e.Payments.Any(p => p.PaymentStatus == filters.PaymentStatus.Value));
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
                TrackPrice = e.TrainingTrack != null ? e.TrainingTrack.Price : 0m,
                EnrollmentDate = e.EnrollmentDate,
                Status = e.Status,
                ProgressPercentage = e.ProgressPercentage,
                FinalResult = e.FinalResult,
                TotalPaid = e.Payments.Where(p => p.PaymentStatus == Common.PaymentStatus.Completed).Sum(p => p.Amount)
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
            .Include(e => e.Payments)
            .FirstOrDefaultAsync(e => e.EnrollmentId == id);

        if (enrollment == null)
        {
            throw new NotFoundException($"Enrollment with ID {id} was not found.");
        }

        var trackPrice = enrollment.TrainingTrack?.Price ?? 0m;
        var totalPaid = enrollment.Payments.Where(p => p.PaymentStatus == Common.PaymentStatus.Completed).Sum(p => p.Amount);

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
            EnrollmentDate = enrollment.EnrollmentDate,
            Status = enrollment.Status,
            ProgressPercentage = enrollment.ProgressPercentage,
            FinalResult = enrollment.FinalResult,
            TotalPaid = totalPaid,
            CreatedAt = enrollment.CreatedAt,
            UpdatedAt = enrollment.UpdatedAt,
            Payments = enrollment.Payments.Select(p => new EnrollmentPaymentSummaryDto
            {
                PaymentId = p.PaymentId,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                PaymentDate = p.PaymentDate,
                PaymentStatus = p.PaymentStatus,
                ReferenceNumber = p.ReferenceNumber,
                Notes = p.Notes
            }).OrderByDescending(p => p.PaymentDate).ToList()
        };
    }

    public async Task<EnrollmentDetailsResponse> CreateEnrollmentAsync(CreateEnrollmentRequest request)
    {
        // 1. Validate Student exists and is active
        var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == request.StudentId);
        if (student == null)
        {
            throw new BadRequestException($"Student with ID {request.StudentId} does not exist.");
        }

        if (!student.IsActive)
        {
            throw new BadRequestException("Cannot enroll an inactive or suspended student.");
        }

        // 2. Validate Track exists and is enrollable
        var track = await _context.TrainingTracks
            .Include(t => t.Enrollments)
            .FirstOrDefaultAsync(t => t.TrainingTrackId == request.TrainingTrackId);

        if (track == null)
        {
            throw new BadRequestException($"Training Track with ID {request.TrainingTrackId} does not exist.");
        }

        if (track.Status == Common.TrackStatus.Completed || track.Status == Common.TrackStatus.Cancelled)
        {
            throw new BadRequestException($"Cannot enroll in track with status '{track.Status}'.");
        }

        // 3. Prevent duplicate enrollment
        var alreadyEnrolled = await _context.Enrollments.AnyAsync(e => e.StudentId == request.StudentId && e.TrainingTrackId == request.TrainingTrackId);
        if (alreadyEnrolled)
        {
            throw new ConflictException($"Student '{student.FullName}' is already enrolled in track '{track.Title}'.");
        }

        // 4. Validate Capacity
        var currentActiveCount = track.Enrollments.Count(e => e.Status == Common.EnrollmentStatus.Active || e.Status == Common.EnrollmentStatus.Pending);
        if (currentActiveCount >= track.Capacity)
        {
            throw new BadRequestException($"Training track '{track.Title}' has reached its maximum capacity of {track.Capacity} students.");
        }

        var enrollment = new Enrollment
        {
            StudentId = request.StudentId,
            TrainingTrackId = request.TrainingTrackId,
            EnrollmentDate = DateTime.UtcNow,
            Status = request.Status,
            ProgressPercentage = 0.00m,
            CreatedAt = DateTime.UtcNow
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

        if (!string.IsNullOrWhiteSpace(request.FinalResult))
        {
            enrollment.FinalResult = request.FinalResult.Trim();
        }

        enrollment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return await GetEnrollmentByIdAsync(enrollment.EnrollmentId);
    }
}
