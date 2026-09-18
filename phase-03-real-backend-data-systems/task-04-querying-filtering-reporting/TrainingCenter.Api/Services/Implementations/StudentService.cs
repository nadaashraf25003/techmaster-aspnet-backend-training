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
                EnrollmentsCount = s.Enrollments.Count,
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
            EnrollmentsCount = student.Enrollments.Count,
            CreatedAt = student.CreatedAt
        };
    }

    public async Task<StudentResponse> CreateStudentAsync(CreateStudentRequest request)
    {
        var emailExists = await _context.Students.AnyAsync(s => s.Email == request.Email);
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

        var emailConflict = await _context.Students.AnyAsync(s => s.Email == request.Email && s.StudentId != id);
        if (emailConflict)
        {
            throw new ConflictException($"A student with email '{request.Email}' already exists.");
        }

        student.FullName = request.FullName;
        student.Email = request.Email;
        student.PhoneNumber = request.PhoneNumber;
        student.DateOfBirth = request.DateOfBirth;
        student.Address = request.Address;
        student.IsActive = request.IsActive;

        await _context.SaveChangesAsync();
        return await GetStudentByIdAsync(id);
    }

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
