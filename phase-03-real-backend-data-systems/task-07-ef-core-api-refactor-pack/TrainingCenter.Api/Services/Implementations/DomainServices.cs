using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common;
using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.Data;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Instructors;
using TrainingCenter.Api.DTOs.Students;
using TrainingCenter.Api.DTOs.Tracks;
using TrainingCenter.Api.Entities;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Services.Implementations;

public class StudentService : IStudentService
{
    private readonly TrainingCenterDbContext _context;

    public StudentService(TrainingCenterDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<StudentResponseDto>> GetAllPagedAsync(PaginationParams pagination, CancellationToken cancellationToken = default)
    {
        var query = _context.Students.AsNoTracking();
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(s => s.FullName)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(s => new StudentResponseDto
            {
                Id = s.Id,
                FullName = s.FullName,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
                DateOfBirth = s.DateOfBirth,
                Address = s.Address,
                IsActive = s.IsActive,
                EnrollmentsCount = s.Enrollments.Count(e => !e.IsDeleted),
                CreatedAtUtc = s.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<StudentResponseDto>(items, totalCount, pagination.PageNumber, pagination.PageSize);
    }

    public async Task<StudentResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var student = await _context.Students
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new StudentResponseDto
            {
                Id = s.Id,
                FullName = s.FullName,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
                DateOfBirth = s.DateOfBirth,
                Address = s.Address,
                IsActive = s.IsActive,
                EnrollmentsCount = s.Enrollments.Count(e => !e.IsDeleted),
                CreatedAtUtc = s.CreatedAtUtc
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (student == null)
        {
            throw new NotFoundException($"Student with ID {id} was not found.");
        }

        return student;
    }

    public async Task<StudentResponseDto> CreateAsync(CreateStudentRequestDto request, CancellationToken cancellationToken = default)
    {
        var emailExists = await _context.Students.AnyAsync(s => s.Email.ToLower() == request.Email.ToLower(), cancellationToken);
        if (emailExists)
        {
            throw new ConflictException($"A student with email '{request.Email}' already exists.");
        }

        var student = new Student
        {
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth,
            Address = request.Address,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _context.Students.AddAsync(student, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new StudentResponseDto
        {
            Id = student.Id,
            FullName = student.FullName,
            Email = student.Email,
            PhoneNumber = student.PhoneNumber,
            DateOfBirth = student.DateOfBirth,
            Address = student.Address,
            IsActive = student.IsActive,
            EnrollmentsCount = 0,
            CreatedAtUtc = student.CreatedAtUtc
        };
    }

    public async Task<StudentResponseDto> UpdateAsync(int id, UpdateStudentRequestDto request, CancellationToken cancellationToken = default)
    {
        var student = await _context.Students.FindAsync(new object[] { id }, cancellationToken);
        if (student == null)
        {
            throw new NotFoundException($"Student with ID {id} was not found.");
        }

        student.FullName = request.FullName;
        student.PhoneNumber = request.PhoneNumber;
        student.DateOfBirth = request.DateOfBirth;
        student.Address = request.Address;
        student.IsActive = request.IsActive;
        student.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new StudentResponseDto
        {
            Id = student.Id,
            FullName = student.FullName,
            Email = student.Email,
            PhoneNumber = student.PhoneNumber,
            DateOfBirth = student.DateOfBirth,
            Address = student.Address,
            IsActive = student.IsActive,
            EnrollmentsCount = student.Enrollments.Count(e => !e.IsDeleted),
            CreatedAtUtc = student.CreatedAtUtc
        };
    }

    public async Task SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var student = await _context.Students.FindAsync(new object[] { id }, cancellationToken);
        if (student == null)
        {
            throw new NotFoundException($"Student with ID {id} was not found.");
        }

        student.IsDeleted = true;
        student.DeletedAtUtc = DateTime.UtcNow;
        student.UpdatedAtUtc = DateTime.UtcNow;
        student.IsActive = false;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

public class TrackService : ITrackService
{
    private readonly TrainingCenterDbContext _context;

    public TrackService(TrainingCenterDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<TrackResponseDto>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TrainingTracks
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new TrackResponseDto
            {
                Id = t.Id,
                Code = t.Code,
                Name = t.Name,
                Description = t.Description,
                Price = t.Price,
                Capacity = t.Capacity,
                Status = t.Status,
                InstructorId = t.InstructorId,
                InstructorName = t.Instructor != null ? t.Instructor.FullName : null,
                ActiveEnrollmentsCount = t.Enrollments.Count(e => e.Status == EnrollmentStatus.Active && !e.IsDeleted),
                CreatedAtUtc = t.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<TrackResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var track = await _context.TrainingTracks
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new TrackResponseDto
            {
                Id = t.Id,
                Code = t.Code,
                Name = t.Name,
                Description = t.Description,
                Price = t.Price,
                Capacity = t.Capacity,
                Status = t.Status,
                InstructorId = t.InstructorId,
                InstructorName = t.Instructor != null ? t.Instructor.FullName : null,
                ActiveEnrollmentsCount = t.Enrollments.Count(e => e.Status == EnrollmentStatus.Active && !e.IsDeleted),
                CreatedAtUtc = t.CreatedAtUtc
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (track == null)
        {
            throw new NotFoundException($"Track with ID {id} was not found.");
        }

        return track;
    }

    public async Task<TrackResponseDto> CreateAsync(CreateTrackRequestDto request, CancellationToken cancellationToken = default)
    {
        var codeExists = await _context.TrainingTracks.AnyAsync(t => t.Code.ToLower() == request.Code.ToLower(), cancellationToken);
        if (codeExists)
        {
            throw new ConflictException($"Track code '{request.Code}' is already registered.");
        }

        var track = new TrainingTrack
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Capacity = request.Capacity,
            Status = request.Status,
            InstructorId = request.InstructorId,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _context.TrainingTracks.AddAsync(track, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new TrackResponseDto
        {
            Id = track.Id,
            Code = track.Code,
            Name = track.Name,
            Description = track.Description,
            Price = track.Price,
            Capacity = track.Capacity,
            Status = track.Status,
            InstructorId = track.InstructorId,
            ActiveEnrollmentsCount = 0,
            CreatedAtUtc = track.CreatedAtUtc
        };
    }
}

public class InstructorService : IInstructorService
{
    private readonly TrainingCenterDbContext _context;

    public InstructorService(TrainingCenterDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<InstructorResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Instructors
            .AsNoTracking()
            .OrderBy(i => i.FullName)
            .Select(i => new InstructorResponseDto
            {
                Id = i.Id,
                FullName = i.FullName,
                Email = i.Email,
                PhoneNumber = i.PhoneNumber,
                Specialization = i.Specialization,
                HourlyRate = i.HourlyRate,
                IsActive = i.IsActive,
                AssignedTracksCount = i.TrainingTracks.Count(t => !t.IsDeleted),
                CreatedAtUtc = i.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<InstructorResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var instructor = await _context.Instructors
            .AsNoTracking()
            .Where(i => i.Id == id)
            .Select(i => new InstructorResponseDto
            {
                Id = i.Id,
                FullName = i.FullName,
                Email = i.Email,
                PhoneNumber = i.PhoneNumber,
                Specialization = i.Specialization,
                HourlyRate = i.HourlyRate,
                IsActive = i.IsActive,
                AssignedTracksCount = i.TrainingTracks.Count(t => !t.IsDeleted),
                CreatedAtUtc = i.CreatedAtUtc
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (instructor == null)
        {
            throw new NotFoundException($"Instructor with ID {id} was not found.");
        }

        return instructor;
    }

    public async Task<InstructorResponseDto> CreateAsync(CreateInstructorRequestDto request, CancellationToken cancellationToken = default)
    {
        var emailExists = await _context.Instructors.AnyAsync(i => i.Email.ToLower() == request.Email.ToLower(), cancellationToken);
        if (emailExists)
        {
            throw new ConflictException($"Instructor with email '{request.Email}' already exists.");
        }

        var instructor = new Instructor
        {
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Specialization = request.Specialization,
            HourlyRate = request.HourlyRate,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _context.Instructors.AddAsync(instructor, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new InstructorResponseDto
        {
            Id = instructor.Id,
            FullName = instructor.FullName,
            Email = instructor.Email,
            PhoneNumber = instructor.PhoneNumber,
            Specialization = instructor.Specialization,
            HourlyRate = instructor.HourlyRate,
            IsActive = instructor.IsActive,
            AssignedTracksCount = 0,
            CreatedAtUtc = instructor.CreatedAtUtc
        };
    }
}
