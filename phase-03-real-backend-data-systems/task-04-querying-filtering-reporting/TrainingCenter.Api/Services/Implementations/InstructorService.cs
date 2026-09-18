using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.Data;
using TrainingCenter.Api.DTOs.Common;
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
                AssignedTracksCount = i.TrainingTracks.Count,
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
            AssignedTracksCount = instructor.TrainingTracks.Count,
            CreatedAt = instructor.CreatedAt
        };
    }

    public async Task<InstructorResponse> CreateInstructorAsync(CreateInstructorRequest request)
    {
        var emailExists = await _context.Instructors.AnyAsync(i => i.Email == request.Email);
        if (emailExists)
        {
            throw new ConflictException($"An instructor with email '{request.Email}' already exists.");
        }

        var instructor = new Instructor
        {
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Specialization = request.Specialization,
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

        var emailConflict = await _context.Instructors.AnyAsync(i => i.Email == request.Email && i.InstructorId != id);
        if (emailConflict)
        {
            throw new ConflictException($"An instructor with email '{request.Email}' already exists.");
        }

        instructor.FullName = request.FullName;
        instructor.Email = request.Email;
        instructor.PhoneNumber = request.PhoneNumber;
        instructor.Specialization = request.Specialization;
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
}
