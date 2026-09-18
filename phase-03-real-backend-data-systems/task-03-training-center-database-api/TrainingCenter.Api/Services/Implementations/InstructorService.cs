using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.Data;
using TrainingCenter.Api.DTOs.Instructors;
using TrainingCenter.Api.Entities;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Services.Implementations;

public class InstructorService : IInstructorService
{
    private readonly TrainingCenterDbContext _context;

    public InstructorService(TrainingCenterDbContext context)
    {
        _context = context;
    }

    public async Task<List<InstructorResponse>> GetAllInstructorsAsync(bool? isActive = null)
    {
        var query = _context.Instructors.AsNoTracking().AsQueryable();

        if (isActive.HasValue)
        {
            query = query.Where(i => i.IsActive == isActive.Value);
        }

        return await query
            .OrderBy(i => i.FullName)
            .Select(i => new InstructorResponse
            {
                InstructorId = i.InstructorId,
                FullName = i.FullName,
                Email = i.Email,
                Specialization = i.Specialization,
                Bio = i.Bio,
                IsActive = i.IsActive,
                ActiveTracksCount = i.TrainingTracks.Count(t => t.Status == Common.TrackStatus.InProgress || t.Status == Common.TrackStatus.Upcoming),
                CreatedAt = i.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<InstructorDetailsResponse> GetInstructorByIdAsync(int id)
    {
        var instructor = await _context.Instructors
            .AsNoTracking()
            .Include(i => i.TrainingTracks)
                .ThenInclude(t => t.Enrollments)
            .FirstOrDefaultAsync(i => i.InstructorId == id);

        if (instructor == null)
        {
            throw new NotFoundException($"Instructor with ID {id} was not found.");
        }

        return new InstructorDetailsResponse
        {
            InstructorId = instructor.InstructorId,
            FullName = instructor.FullName,
            Email = instructor.Email,
            Specialization = instructor.Specialization,
            Bio = instructor.Bio,
            IsActive = instructor.IsActive,
            CreatedAt = instructor.CreatedAt,
            UpdatedAt = instructor.UpdatedAt,
            Tracks = instructor.TrainingTracks.Select(t => new InstructorTrackSummaryDto
            {
                TrainingTrackId = t.TrainingTrackId,
                Title = t.Title,
                Code = t.Code,
                Level = t.Level.ToString(),
                Status = t.Status.ToString(),
                Capacity = t.Capacity,
                EnrolledStudentsCount = t.Enrollments.Count,
                StartDate = t.StartDate,
                EndDate = t.EndDate
            }).ToList()
        };
    }

    public async Task<InstructorResponse> CreateInstructorAsync(CreateInstructorRequest request)
    {
        var emailExists = await _context.Instructors.AnyAsync(i => i.Email.ToLower() == request.Email.Trim().ToLower());
        if (emailExists)
        {
            throw new ConflictException($"An instructor with email '{request.Email}' already exists.");
        }

        var instructor = new Instructor
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim().ToLower(),
            Specialization = request.Specialization.Trim(),
            Bio = request.Bio?.Trim(),
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Instructors.AddAsync(instructor);
        await _context.SaveChangesAsync();

        return new InstructorResponse
        {
            InstructorId = instructor.InstructorId,
            FullName = instructor.FullName,
            Email = instructor.Email,
            Specialization = instructor.Specialization,
            Bio = instructor.Bio,
            IsActive = instructor.IsActive,
            ActiveTracksCount = 0,
            CreatedAt = instructor.CreatedAt
        };
    }

    public async Task<InstructorResponse> UpdateInstructorAsync(int id, UpdateInstructorRequest request)
    {
        var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.InstructorId == id);
        if (instructor == null)
        {
            throw new NotFoundException($"Instructor with ID {id} was not found.");
        }

        if (instructor.Email.ToLower() != request.Email.Trim().ToLower())
        {
            var emailExists = await _context.Instructors.AnyAsync(i => i.Email.ToLower() == request.Email.Trim().ToLower() && i.InstructorId != id);
            if (emailExists)
            {
                throw new ConflictException($"An instructor with email '{request.Email}' already exists.");
            }
        }

        instructor.FullName = request.FullName.Trim();
        instructor.Email = request.Email.Trim().ToLower();
        instructor.Specialization = request.Specialization.Trim();
        instructor.Bio = request.Bio?.Trim();
        instructor.IsActive = request.IsActive;
        instructor.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var activeTracks = await _context.TrainingTracks.CountAsync(t => t.InstructorId == id && (t.Status == Common.TrackStatus.InProgress || t.Status == Common.TrackStatus.Upcoming));

        return new InstructorResponse
        {
            InstructorId = instructor.InstructorId,
            FullName = instructor.FullName,
            Email = instructor.Email,
            Specialization = instructor.Specialization,
            Bio = instructor.Bio,
            IsActive = instructor.IsActive,
            ActiveTracksCount = activeTracks,
            CreatedAt = instructor.CreatedAt
        };
    }

    public async Task<List<InstructorTrackSummaryDto>> GetInstructorTracksAsync(int instructorId)
    {
        var instructorExists = await _context.Instructors.AnyAsync(i => i.InstructorId == instructorId);
        if (!instructorExists)
        {
            throw new NotFoundException($"Instructor with ID {instructorId} was not found.");
        }

        return await _context.TrainingTracks
            .AsNoTracking()
            .Where(t => t.InstructorId == instructorId)
            .Select(t => new InstructorTrackSummaryDto
            {
                TrainingTrackId = t.TrainingTrackId,
                Title = t.Title,
                Code = t.Code,
                Level = t.Level.ToString(),
                Status = t.Status.ToString(),
                Capacity = t.Capacity,
                EnrolledStudentsCount = t.Enrollments.Count,
                StartDate = t.StartDate,
                EndDate = t.EndDate
            })
            .ToListAsync();
    }
}
