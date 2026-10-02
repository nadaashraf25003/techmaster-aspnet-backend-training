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

    public async Task<PagedResult<EnrollmentListItemResponse>> GetEnrollmentsAsync(EnrollmentFilterParams filters)
    {
        var query = _context.Enrollments
            .AsNoTracking()
            .Include(e => e.Student)
            .Include(e => e.TrainingTrack)
            .Include(e => e.Payments)
            .AsQueryable();

        if (filters.TrackId.HasValue)
        {
            query = query.Where(e => e.TrainingTrackId == filters.TrackId.Value);
        }

        if (filters.StudentId.HasValue)
        {
            query = query.Where(e => e.StudentId == filters.StudentId.Value);
        }

        if (filters.Status.HasValue)
        {
            query = query.Where(e => e.Status == filters.Status.Value);
        }

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
        }

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

    /// <summary>
    /// Business Rules Enforced:
    /// - Rule S5: Inactive/deleted students cannot receive new enrollment.
    /// - Rule T6: Closed/Completed/Draft/Archived tracks reject new enrollments.
    /// - Rule E1: Duplicate active/pending enrollments in the same track are rejected.
    /// - Rule T5 and E5: Track capacity cannot be exceeded (ignoring cancelled enrollments).
    /// - Rule E2: New enrollment starts as Pending by default.
    /// </summary>
    public async Task<EnrollmentDetailsResponse> CreateEnrollmentAsync(CreateEnrollmentRequest request)
    {
        // 1. Verify Student Existence & Active Status (Rule S5)
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.StudentId == request.StudentId);

        if (student == null)
        {
            throw new BadRequestException($"Student with ID {request.StudentId} was not found.");
        }

        if (!student.IsActive)
        {
            throw new BadRequestException($"Cannot enroll student '{student.FullName}' because their account is inactive.");
        }

        // 2. Verify Track Existence & Status (Rule T6)
        var track = await _context.TrainingTracks
            .Include(t => t.Enrollments)
            .FirstOrDefaultAsync(t => t.TrainingTrackId == request.TrainingTrackId);

        if (track == null)
        {
            throw new BadRequestException($"Training Track with ID {request.TrainingTrackId} was not found.");
        }

        if (track.Status is TrackStatus.Closed or TrackStatus.Completed or TrackStatus.Archived or TrackStatus.Draft)
        {
            throw new BadRequestException($"Cannot enroll in track '{track.Title}' with status '{track.Status}'. Only Upcoming or InProgress tracks accept enrollments.");
        }

        // 3. Rule E1: Student cannot have duplicate active/pending enrollment in the same track
        var existingActiveEnrollment = await _context.Enrollments
            .AnyAsync(e => e.StudentId == request.StudentId 
                        && e.TrainingTrackId == request.TrainingTrackId 
                        && (e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Pending));

        if (existingActiveEnrollment)
        {
            throw new BadRequestException($"Student '{student.FullName}' is already enrolled in track '{track.Title}' with an active or pending status.");
        }

        // 4. Rule T5 & E5: Capacity enforcement (Cancelled enrollments do NOT count in capacity)
        var activeOrPendingCount = track.Enrollments
            .Count(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Pending);

        if (activeOrPendingCount >= track.Capacity)
        {
            throw new BadRequestException($"Track '{track.Title}' has reached its maximum capacity of {track.Capacity} students.");
        }

        // 5. Rule E2: Newly created enrollment starts as Pending by default
        var enrollment = new Enrollment
        {
            StudentId = request.StudentId,
            TrainingTrackId = request.TrainingTrackId,
            EnrollmentDate = DateTime.UtcNow,
            Status = EnrollmentStatus.Pending,
            Notes = request.Notes
        };

        await _context.Enrollments.AddAsync(enrollment);
        await _context.SaveChangesAsync();

        return await GetEnrollmentByIdAsync(enrollment.EnrollmentId);
    }

    /// <summary>
    /// Business Rules Enforced:
    /// - Rule E4: Completed enrollment cannot be cancelled directly.
    /// </summary>
    public async Task<EnrollmentDetailsResponse> UpdateEnrollmentStatusAsync(int id, UpdateEnrollmentStatusRequest request)
    {
        var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.EnrollmentId == id);
        if (enrollment == null)
        {
            throw new NotFoundException($"Enrollment with ID {id} was not found.");
        }

        if (!Enum.IsDefined(typeof(EnrollmentStatus), request.Status))
        {
            throw new BadRequestException($"Invalid enrollment status value '{request.Status}'.");
        }

        // Rule E4: Completed enrollment cannot be cancelled directly
        if (enrollment.Status == EnrollmentStatus.Completed && request.Status == EnrollmentStatus.Cancelled)
        {
            throw new BadRequestException("Completed enrollments cannot be cancelled directly. Please contact an administrator.");
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
