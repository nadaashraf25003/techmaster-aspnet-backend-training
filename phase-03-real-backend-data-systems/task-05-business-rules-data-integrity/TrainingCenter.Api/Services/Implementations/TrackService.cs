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

    public async Task<PagedResult<TrackResponse>> GetTracksAsync(TrackFilterParams filters)
    {
        var query = _context.TrainingTracks
            .AsNoTracking()
            .Include(t => t.PrimaryInstructor)
            .Include(t => t.Enrollments)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var search = filters.Search.Trim().ToLower();
            query = query.Where(t => t.Title.ToLower().Contains(search) || t.Code.ToLower().Contains(search));
        }

        if (filters.Status.HasValue)
        {
            query = query.Where(t => t.Status == filters.Status.Value);
        }

        if (filters.InstructorId.HasValue)
        {
            query = query.Where(t => t.PrimaryInstructorId == filters.InstructorId.Value);
        }

        if (filters.MinPrice.HasValue)
        {
            query = query.Where(t => t.Price >= filters.MinPrice.Value);
        }

        if (filters.MaxPrice.HasValue)
        {
            query = query.Where(t => t.Price <= filters.MaxPrice.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(t => t.Title)
            .Skip((filters.PageNumber - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .Select(t => new TrackResponse
            {
                TrainingTrackId = t.TrainingTrackId,
                Title = t.Title,
                Code = t.Code,
                Description = t.Description,
                Price = t.Price,
                DurationHours = t.DurationHours,
                Capacity = t.Capacity,
                Status = t.Status,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                PrimaryInstructorId = t.PrimaryInstructorId,
                InstructorName = t.PrimaryInstructor != null ? t.PrimaryInstructor.FullName : null,
                ActiveEnrollmentsCount = t.Enrollments.Count(e => e.Status == Common.EnrollmentStatus.Active || e.Status == Common.EnrollmentStatus.Pending),
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<TrackResponse>(items, totalCount, filters.PageNumber, filters.PageSize);
    }

    public async Task<TrackResponse> GetTrackByIdAsync(int id)
    {
        var track = await _context.TrainingTracks
            .AsNoTracking()
            .Include(t => t.PrimaryInstructor)
            .Include(t => t.Enrollments)
            .FirstOrDefaultAsync(t => t.TrainingTrackId == id);

        if (track == null)
        {
            throw new NotFoundException($"Training Track with ID {id} was not found.");
        }

        return new TrackResponse
        {
            TrainingTrackId = track.TrainingTrackId,
            Title = track.Title,
            Code = track.Code,
            Description = track.Description,
            Price = track.Price,
            DurationHours = track.DurationHours,
            Capacity = track.Capacity,
            Status = track.Status,
            StartDate = track.StartDate,
            EndDate = track.EndDate,
            PrimaryInstructorId = track.PrimaryInstructorId,
            InstructorName = track.PrimaryInstructor?.FullName,
            ActiveEnrollmentsCount = track.Enrollments.Count(e => e.Status == Common.EnrollmentStatus.Active || e.Status == Common.EnrollmentStatus.Pending),
            CreatedAt = track.CreatedAt
        };
    }

    /// <summary>
    /// Rule T1: Title required, Code unique.
    /// Rule T2: Capacity must be greater than 0.
    /// Rule T3: StartDate must be before EndDate.
    /// Rule T4: Primary Instructor is required and must be active.
    /// </summary>
    public async Task<TrackResponse> CreateTrackAsync(CreateTrackRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new BadRequestException("Track Title is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Code))
        {
            throw new BadRequestException("Track Code is required.");
        }

        // Rule T2: Capacity must be > 0
        if (request.Capacity <= 0)
        {
            throw new BadRequestException("Track Capacity must be strictly greater than 0.");
        }

        // Rule T3: StartDate < EndDate
        if (request.StartDate >= request.EndDate)
        {
            throw new BadRequestException($"Invalid track dates: StartDate ({request.StartDate:yyyy-MM-dd}) must be before EndDate ({request.EndDate:yyyy-MM-dd}).");
        }

        // Rule T4: Instructor is required
        if (!request.PrimaryInstructorId.HasValue || request.PrimaryInstructorId.Value <= 0)
        {
            throw new BadRequestException("Primary Instructor is required when creating a training track.");
        }

        var instructor = await _context.Instructors
            .FirstOrDefaultAsync(i => i.InstructorId == request.PrimaryInstructorId.Value);

        if (instructor == null)
        {
            throw new BadRequestException($"Instructor with ID {request.PrimaryInstructorId.Value} was not found.");
        }

        if (!instructor.IsActive)
        {
            throw new BadRequestException($"Cannot assign instructor '{instructor.FullName}' because their account is inactive.");
        }

        var codeNormalized = request.Code.Trim().ToUpperInvariant();
        var codeExists = await _context.TrainingTracks.AnyAsync(t => t.Code.ToUpper() == codeNormalized);
        if (codeExists)
        {
            throw new BadRequestException($"A training track with code '{request.Code}' already exists. Track Code must be unique.");
        }

        var track = new TrainingTrack
        {
            Title = request.Title.Trim(),
            Code = codeNormalized,
            Description = request.Description?.Trim() ?? string.Empty,
            Price = request.Price,
            DurationHours = request.DurationHours,
            Capacity = request.Capacity,
            Status = request.Status,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            PrimaryInstructorId = request.PrimaryInstructorId
        };

        await _context.TrainingTracks.AddAsync(track);
        await _context.SaveChangesAsync();

        return await GetTrackByIdAsync(track.TrainingTrackId);
    }

    public async Task<TrackResponse> UpdateTrackAsync(int id, UpdateTrackRequest request)
    {
        var track = await _context.TrainingTracks.FirstOrDefaultAsync(t => t.TrainingTrackId == id);
        if (track == null)
        {
            throw new NotFoundException($"Training Track with ID {id} was not found.");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new BadRequestException("Track Title is required.");
        }

        // Rule T2: Capacity must be > 0
        if (request.Capacity <= 0)
        {
            throw new BadRequestException("Track Capacity must be strictly greater than 0.");
        }

        // Rule T3: StartDate < EndDate
        if (request.StartDate >= request.EndDate)
        {
            throw new BadRequestException($"Invalid track dates: StartDate ({request.StartDate:yyyy-MM-dd}) must be before EndDate ({request.EndDate:yyyy-MM-dd}).");
        }

        // Rule T4: Instructor is required
        if (!request.PrimaryInstructorId.HasValue || request.PrimaryInstructorId.Value <= 0)
        {
            throw new BadRequestException("Primary Instructor is required.");
        }

        var instructor = await _context.Instructors
            .FirstOrDefaultAsync(i => i.InstructorId == request.PrimaryInstructorId.Value);

        if (instructor == null)
        {
            throw new BadRequestException($"Instructor with ID {request.PrimaryInstructorId.Value} was not found.");
        }

        if (!instructor.IsActive)
        {
            throw new BadRequestException($"Cannot assign instructor '{instructor.FullName}' because their account is inactive.");
        }

        track.Title = request.Title.Trim();
        track.Description = request.Description?.Trim() ?? string.Empty;
        track.Price = request.Price;
        track.DurationHours = request.DurationHours;
        track.Capacity = request.Capacity;
        track.Status = request.Status;
        track.StartDate = request.StartDate;
        track.EndDate = request.EndDate;
        track.PrimaryInstructorId = request.PrimaryInstructorId;

        await _context.SaveChangesAsync();
        return await GetTrackByIdAsync(id);
    }

    public async Task DeleteTrackAsync(int id)
    {
        var track = await _context.TrainingTracks.FirstOrDefaultAsync(t => t.TrainingTrackId == id);
        if (track == null)
        {
            throw new NotFoundException($"Training Track with ID {id} was not found.");
        }

        track.IsDeleted = true;
        await _context.SaveChangesAsync();
    }
}
