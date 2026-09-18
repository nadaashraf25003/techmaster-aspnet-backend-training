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
                EnrolledStudentsCount = t.Enrollments.Count(e => e.Status == Common.EnrollmentStatus.Active || e.Status == Common.EnrollmentStatus.Pending),
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
            EnrolledStudentsCount = track.Enrollments.Count(e => e.Status == Common.EnrollmentStatus.Active || e.Status == Common.EnrollmentStatus.Pending),
            CreatedAt = track.CreatedAt
        };
    }

    public async Task<TrackResponse> CreateTrackAsync(CreateTrackRequest request)
    {
        var codeExists = await _context.TrainingTracks.AnyAsync(t => t.Code == request.Code);
        if (codeExists)
        {
            throw new ConflictException($"A track with code '{request.Code}' already exists.");
        }

        if (request.PrimaryInstructorId.HasValue)
        {
            var instructorExists = await _context.Instructors.AnyAsync(i => i.InstructorId == request.PrimaryInstructorId.Value);
            if (!instructorExists)
            {
                throw new NotFoundException($"Instructor with ID {request.PrimaryInstructorId.Value} was not found.");
            }
        }

        var track = new TrainingTrack
        {
            Title = request.Title,
            Code = request.Code,
            Description = request.Description,
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

        if (request.PrimaryInstructorId.HasValue)
        {
            var instructorExists = await _context.Instructors.AnyAsync(i => i.InstructorId == request.PrimaryInstructorId.Value);
            if (!instructorExists)
            {
                throw new NotFoundException($"Instructor with ID {request.PrimaryInstructorId.Value} was not found.");
            }
        }

        track.Title = request.Title;
        track.Description = request.Description;
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
