using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.Data;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Tracks;
using TrainingCenter.Api.Entities;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Services.Implementations;

public class TrackService : ITrackService
{
    private readonly TrainingCenterDbContext _context;

    public TrackService(TrainingCenterDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<TrackListItemResponse>> GetTracksAsync(TrackFilterParams filters)
    {
        var query = _context.TrainingTracks
            .AsNoTracking()
            .Include(t => t.Instructor)
            .Include(t => t.Enrollments)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filters.Keyword))
        {
            var keyword = filters.Keyword.Trim().ToLower();
            query = query.Where(t => t.Title.ToLower().Contains(keyword) || t.Code.ToLower().Contains(keyword));
        }

        if (filters.Level.HasValue)
        {
            query = query.Where(t => t.Level == filters.Level.Value);
        }

        if (filters.Status.HasValue)
        {
            query = query.Where(t => t.Status == filters.Status.Value);
        }

        if (filters.InstructorId.HasValue)
        {
            query = query.Where(t => t.InstructorId == filters.InstructorId.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(t => t.StartDate)
            .Skip((filters.PageNumber - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .Select(t => new TrackListItemResponse
            {
                TrainingTrackId = t.TrainingTrackId,
                Title = t.Title,
                Code = t.Code,
                Level = t.Level,
                Price = t.Price,
                Capacity = t.Capacity,
                EnrolledStudentsCount = t.Enrollments.Count(e => e.Status == Common.EnrollmentStatus.Active || e.Status == Common.EnrollmentStatus.Pending),
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                Status = t.Status,
                InstructorId = t.InstructorId,
                InstructorName = t.Instructor != null ? t.Instructor.FullName : string.Empty
            })
            .ToListAsync();

        return new PagedResult<TrackListItemResponse>(items, totalCount, filters.PageNumber, filters.PageSize);
    }

    public async Task<TrackDetailsResponse> GetTrackByIdAsync(int id)
    {
        var track = await _context.TrainingTracks
            .AsNoTracking()
            .Include(t => t.Instructor)
            .Include(t => t.Enrollments)
                .ThenInclude(e => e.Student)
            .FirstOrDefaultAsync(t => t.TrainingTrackId == id);

        if (track == null)
        {
            throw new NotFoundException($"Training Track with ID {id} was not found.");
        }

        var enrolledCount = track.Enrollments.Count(e => e.Status == Common.EnrollmentStatus.Active || e.Status == Common.EnrollmentStatus.Pending);

        return new TrackDetailsResponse
        {
            TrainingTrackId = track.TrainingTrackId,
            Title = track.Title,
            Code = track.Code,
            Description = track.Description,
            Level = track.Level,
            Price = track.Price,
            Capacity = track.Capacity,
            EnrolledStudentsCount = enrolledCount,
            StartDate = track.StartDate,
            EndDate = track.EndDate,
            Status = track.Status,
            InstructorId = track.InstructorId,
            InstructorName = track.Instructor?.FullName ?? string.Empty,
            InstructorEmail = track.Instructor?.Email ?? string.Empty,
            EnrolledStudents = track.Enrollments.Select(e => new TrackStudentDto
            {
                StudentId = e.StudentId,
                FullName = e.Student?.FullName ?? string.Empty,
                Email = e.Student?.Email ?? string.Empty,
                PhoneNumber = e.Student?.PhoneNumber,
                EnrollmentId = e.EnrollmentId,
                EnrollmentDate = e.EnrollmentDate,
                Status = e.Status,
                ProgressPercentage = e.ProgressPercentage
            }).ToList()
        };
    }

    public async Task<TrackDetailsResponse> CreateTrackAsync(CreateTrackRequest request)
    {
        // Check date logic
        if (request.EndDate < request.StartDate)
        {
            throw new BadRequestException("EndDate cannot be earlier than StartDate.");
        }

        // Validate instructor exists and is active
        var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.InstructorId == request.InstructorId);
        if (instructor == null)
        {
            throw new BadRequestException($"Instructor with ID {request.InstructorId} does not exist.");
        }

        if (!instructor.IsActive)
        {
            throw new BadRequestException("Cannot assign an inactive instructor to a training track.");
        }

        // Check unique code
        var codeExists = await _context.TrainingTracks.AnyAsync(t => t.Code.ToLower() == request.Code.Trim().ToLower());
        if (codeExists)
        {
            throw new ConflictException($"A training track with code '{request.Code}' already exists.");
        }

        var track = new TrainingTrack
        {
            Title = request.Title.Trim(),
            Code = request.Code.Trim().ToUpper(),
            Description = request.Description?.Trim(),
            Level = request.Level,
            Price = request.Price,
            Capacity = request.Capacity,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = request.Status,
            InstructorId = request.InstructorId,
            CreatedAt = DateTime.UtcNow
        };

        await _context.TrainingTracks.AddAsync(track);
        await _context.SaveChangesAsync();

        return await GetTrackByIdAsync(track.TrainingTrackId);
    }

    public async Task<TrackDetailsResponse> UpdateTrackAsync(int id, UpdateTrackRequest request)
    {
        var track = await _context.TrainingTracks.FirstOrDefaultAsync(t => t.TrainingTrackId == id);
        if (track == null)
        {
            throw new NotFoundException($"Training Track with ID {id} was not found.");
        }

        if (request.EndDate < request.StartDate)
        {
            throw new BadRequestException("EndDate cannot be earlier than StartDate.");
        }

        // Validate instructor
        var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.InstructorId == request.InstructorId);
        if (instructor == null)
        {
            throw new BadRequestException($"Instructor with ID {request.InstructorId} does not exist.");
        }

        if (!instructor.IsActive)
        {
            throw new BadRequestException("Cannot assign an inactive instructor to a training track.");
        }

        // Check unique code if changed
        if (track.Code.ToLower() != request.Code.Trim().ToLower())
        {
            var codeExists = await _context.TrainingTracks.AnyAsync(t => t.Code.ToLower() == request.Code.Trim().ToLower() && t.TrainingTrackId != id);
            if (codeExists)
            {
                throw new ConflictException($"A training track with code '{request.Code}' already exists.");
            }
        }

        // Check capacity against current active enrollments
        var currentEnrolledCount = await _context.Enrollments.CountAsync(e => e.TrainingTrackId == id && (e.Status == Common.EnrollmentStatus.Active || e.Status == Common.EnrollmentStatus.Pending));
        if (request.Capacity < currentEnrolledCount)
        {
            throw new BadRequestException($"Cannot set capacity to {request.Capacity}. The track already has {currentEnrolledCount} enrolled students.");
        }

        track.Title = request.Title.Trim();
        track.Code = request.Code.Trim().ToUpper();
        track.Description = request.Description?.Trim();
        track.Level = request.Level;
        track.Price = request.Price;
        track.Capacity = request.Capacity;
        track.StartDate = request.StartDate;
        track.EndDate = request.EndDate;
        track.Status = request.Status;
        track.InstructorId = request.InstructorId;
        track.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetTrackByIdAsync(track.TrainingTrackId);
    }

    public async Task<bool> SoftDeleteTrackAsync(int id)
    {
        var track = await _context.TrainingTracks
            .Include(t => t.Enrollments)
            .FirstOrDefaultAsync(t => t.TrainingTrackId == id);

        if (track == null)
        {
            throw new NotFoundException($"Training Track with ID {id} was not found.");
        }

        // Check if track has active enrollments
        var hasActiveEnrollments = track.Enrollments.Any(e => e.Status == Common.EnrollmentStatus.Active || e.Status == Common.EnrollmentStatus.Pending);
        if (hasActiveEnrollments)
        {
            throw new BadRequestException("Cannot delete track because it has active student enrollments. Transfer or cancel enrollments first.");
        }

        track.IsDeleted = true;
        track.DeletedAt = DateTime.UtcNow;
        track.Status = Common.TrackStatus.Cancelled;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<TrackStudentDto>> GetTrackStudentsAsync(int trackId)
    {
        var trackExists = await _context.TrainingTracks.AnyAsync(t => t.TrainingTrackId == trackId);
        if (!trackExists)
        {
            throw new NotFoundException($"Training Track with ID {trackId} was not found.");
        }

        return await _context.Enrollments
            .AsNoTracking()
            .Where(e => e.TrainingTrackId == trackId)
            .Include(e => e.Student)
            .OrderBy(e => e.Student != null ? e.Student.FullName : string.Empty)
            .Select(e => new TrackStudentDto
            {
                StudentId = e.StudentId,
                FullName = e.Student != null ? e.Student.FullName : string.Empty,
                Email = e.Student != null ? e.Student.Email : string.Empty,
                PhoneNumber = e.Student != null ? e.Student.PhoneNumber : null,
                EnrollmentId = e.EnrollmentId,
                EnrollmentDate = e.EnrollmentDate,
                Status = e.Status,
                ProgressPercentage = e.ProgressPercentage
            })
            .ToListAsync();
    }
}
