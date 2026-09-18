using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.Data;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Students;
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

    /// <summary>
    /// Rule S4: Returns paginated list of non-deleted students (filtered by global query filter).
    /// </summary>
    public async Task<PagedResult<StudentResponse>> GetStudentsAsync(StudentFilterParams filters)
    {
        var query = _context.Students
            .AsNoTracking()
            .Include(s => s.Enrollments)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var search = filters.Search.Trim().ToLower();
            query = query.Where(s => s.FullName.ToLower().Contains(search) || s.Email.ToLower().Contains(search));
        }

        if (filters.IsActive.HasValue)
        {
            query = query.Where(s => s.IsActive == filters.IsActive.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(s => s.FullName)
            .Skip((filters.PageNumber - 1) * filters.PageSize)
            .Take(filters.PageSize)
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

        return new PagedResult<StudentResponse>(items, totalCount, filters.PageNumber, filters.PageSize);
    }

    public async Task<StudentResponse> GetStudentByIdAsync(int id)
    {
        var student = await _context.Students
            .AsNoTracking()
            .Include(s => s.Enrollments)
            .FirstOrDefaultAsync(s => s.StudentId == id);

        if (student == null)
        {
            throw new NotFoundException($"Student with ID {id} was not found.");
        }

        return new StudentResponse
        {
            StudentId = student.StudentId,
            FullName = student.FullName,
            Email = student.Email,
            PhoneNumber = student.PhoneNumber,
            DateOfBirth = student.DateOfBirth,
            Address = student.Address,
            IsActive = student.IsActive,
            EnrollmentsCount = student.Enrollments.Count(e => e.Status != Common.EnrollmentStatus.Cancelled),
            CreatedAt = student.CreatedAt
        };
    }

    /// <summary>
    /// Rule S1: Email must be unique. Throws BadRequestException on duplicate.
    /// Rule S2: FullName is required.
    /// </summary>
    public async Task<StudentResponse> CreateStudentAsync(CreateStudentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            throw new BadRequestException("Student FullName is required.");
        }

        var emailNormalized = request.Email.Trim().ToLowerInvariant();
        var emailExists = await _context.Students.AnyAsync(s => s.Email.ToLower() == emailNormalized);
        if (emailExists)
        {
            throw new BadRequestException($"A student with email '{request.Email}' already exists. Email must be unique.");
        }

        var student = new Student
        {
            FullName = request.FullName.Trim(),
            Email = emailNormalized,
            PhoneNumber = request.PhoneNumber?.Trim(),
            DateOfBirth = request.DateOfBirth,
            Address = request.Address?.Trim(),
            IsActive = true
        };

        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        return await GetStudentByIdAsync(student.StudentId);
    }

    public async Task<StudentResponse> UpdateStudentAsync(int id, UpdateStudentRequest request)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == id);
        if (student == null)
        {
            throw new NotFoundException($"Student with ID {id} was not found.");
        }

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            throw new BadRequestException("Student FullName is required.");
        }

        var emailNormalized = request.Email.Trim().ToLowerInvariant();
        var emailConflict = await _context.Students.AnyAsync(s => s.Email.ToLower() == emailNormalized && s.StudentId != id);
        if (emailConflict)
        {
            throw new BadRequestException($"A student with email '{request.Email}' already exists. Email must be unique.");
        }

        student.FullName = request.FullName.Trim();
        student.Email = emailNormalized;
        student.PhoneNumber = request.PhoneNumber?.Trim();
        student.DateOfBirth = request.DateOfBirth;
        student.Address = request.Address?.Trim();
        student.IsActive = request.IsActive;

        await _context.SaveChangesAsync();
        return await GetStudentByIdAsync(id);
    }

    /// <summary>
    /// Rule S3: Student cannot be hard deleted by default. Sets IsDeleted = true.
    /// </summary>
    public async Task DeleteStudentAsync(int id)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == id);
        if (student == null)
        {
            throw new NotFoundException($"Student with ID {id} was not found.");
        }

        student.IsDeleted = true;
        await _context.SaveChangesAsync();
    }
}
