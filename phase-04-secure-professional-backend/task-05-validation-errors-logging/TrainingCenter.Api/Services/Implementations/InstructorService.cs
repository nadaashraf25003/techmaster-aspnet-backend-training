using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.Data;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Instructors;
using TrainingCenter.Api.DTOs.Students;
using TrainingCenter.Api.DTOs.Tracks;
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

    public async Task<PagedResult<InstructorResponse>> GetInstructorsAsync(InstructorFilterParams filters)
    {
        var query = _context.Instructors
            .AsNoTracking()
            .Include(i => i.TrainingTracks)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var search = filters.Search.Trim().ToLower();
            query = query.Where(i => i.FullName.ToLower().Contains(search) || i.Email.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(filters.Specialization))
        {
            var spec = filters.Specialization.Trim().ToLower();
            query = query.Where(i => i.Specialization.ToLower().Contains(spec));
        }

        if (filters.IsActive.HasValue)
        {
            query = query.Where(i => i.IsActive == filters.IsActive.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(i => i.FullName)
            .Skip((filters.PageNumber - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .Select(i => new InstructorResponse
            {
                InstructorId = i.InstructorId,
                FullName = i.FullName,
                Email = i.Email,
                PhoneNumber = i.PhoneNumber,
                Specialization = i.Specialization,
                HourlyRate = i.HourlyRate,
                IsActive = i.IsActive,
                AssignedTracksCount = i.TrainingTracks.Count(t => !t.IsDeleted),
                CreatedAt = i.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<InstructorResponse>(items, totalCount, filters.PageNumber, filters.PageSize);
    }

    public async Task<InstructorResponse> GetInstructorByIdAsync(int id)
    {
        var instructor = await _context.Instructors
            .AsNoTracking()
            .Include(i => i.TrainingTracks)
            .FirstOrDefaultAsync(i => i.InstructorId == id);

        if (instructor == null)
        {
            throw new NotFoundException($"Instructor with ID {id} was not found.");
        }

        return new InstructorResponse
        {
            InstructorId = instructor.InstructorId,
            FullName = instructor.FullName,
            Email = instructor.Email,
            PhoneNumber = instructor.PhoneNumber,
            Specialization = instructor.Specialization,
            HourlyRate = instructor.HourlyRate,
            IsActive = instructor.IsActive,
            AssignedTracksCount = instructor.TrainingTracks.Count(t => !t.IsDeleted),
            CreatedAt = instructor.CreatedAt
        };
    }

    public async Task<InstructorResponse> CreateInstructorAsync(CreateInstructorRequest request)
    {
        var emailNormalized = request.Email.Trim().ToLowerInvariant();
        var emailExists = await _context.Instructors.AnyAsync(i => i.Email.ToLower() == emailNormalized);
        if (emailExists)
        {
            throw new BadRequestException($"An instructor with email '{request.Email}' already exists.");
        }

        var instructor = new Instructor
        {
            FullName = request.FullName.Trim(),
            Email = emailNormalized,
            PhoneNumber = request.PhoneNumber?.Trim(),
            Specialization = request.Specialization.Trim(),
            HourlyRate = request.HourlyRate,
            IsActive = true
        };

        await _context.Instructors.AddAsync(instructor);
        await _context.SaveChangesAsync();

        return await GetInstructorByIdAsync(instructor.InstructorId);
    }

    public async Task<InstructorResponse> UpdateInstructorAsync(int id, UpdateInstructorRequest request)
    {
        var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.InstructorId == id);
        if (instructor == null)
        {
            throw new NotFoundException($"Instructor with ID {id} was not found.");
        }

        var emailNormalized = request.Email.Trim().ToLowerInvariant();
        var emailConflict = await _context.Instructors.AnyAsync(i => i.Email.ToLower() == emailNormalized && i.InstructorId != id);
        if (emailConflict)
        {
            throw new BadRequestException($"An instructor with email '{request.Email}' already exists.");
        }

        instructor.FullName = request.FullName.Trim();
        instructor.Email = emailNormalized;
        instructor.PhoneNumber = request.PhoneNumber?.Trim();
        instructor.Specialization = request.Specialization.Trim();
        instructor.HourlyRate = request.HourlyRate;
        instructor.IsActive = request.IsActive;

        await _context.SaveChangesAsync();
        return await GetInstructorByIdAsync(id);
    }

    public async Task DeleteInstructorAsync(int id)
    {
        var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.InstructorId == id);
        if (instructor == null)
        {
            throw new NotFoundException($"Instructor with ID {id} was not found.");
        }

        instructor.IsDeleted = true;
        await _context.SaveChangesAsync();
    }

    public async Task<List<TrackResponse>> GetInstructorTracksAsync(int instructorId)
    {
        var instructorExists = await _context.Instructors.AnyAsync(i => i.InstructorId == instructorId);
        if (!instructorExists)
        {
            throw new NotFoundException($"Instructor with ID {instructorId} was not found.");
        }

        var tracks = await _context.TrainingTracks
            .AsNoTracking()
            .Include(t => t.PrimaryInstructor)
            .Include(t => t.Enrollments)
            .Where(t => t.PrimaryInstructorId == instructorId && !t.IsDeleted)
            .OrderBy(t => t.Title)
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
                ActiveEnrollmentsCount = t.Enrollments.Count(e => e.Status == Common.EnrollmentStatus.Active),
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();

        return tracks;
    }

    public async Task<List<StudentResponse>> GetInstructorStudentsAsync(int instructorId)
    {
        var instructorExists = await _context.Instructors.AnyAsync(i => i.InstructorId == instructorId);
        if (!instructorExists)
        {
            throw new NotFoundException($"Instructor with ID {instructorId} was not found.");
        }

        var students = await _context.Enrollments
            .AsNoTracking()
            .Where(e => e.TrainingTrack != null && e.TrainingTrack.PrimaryInstructorId == instructorId && !e.IsDeleted && e.Student != null && !e.Student.IsDeleted)
            .Select(e => e.Student!)
            .Distinct()
            .OrderBy(s => s.FullName)
            .Select(s => new StudentResponse
            {
                StudentId = s.StudentId,
                FullName = s.FullName,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
                DateOfBirth = s.DateOfBirth,
                Address = s.Address,
                IsActive = s.IsActive,
                EnrollmentsCount = s.Enrollments.Count(e => e.Status != Common.EnrollmentStatus.Cancelled),
                CreatedAt = s.CreatedAt
            })
            .ToListAsync();

        return students;
    }
}
