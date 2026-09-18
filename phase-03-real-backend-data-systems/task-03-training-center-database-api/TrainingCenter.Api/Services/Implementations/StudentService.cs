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

    public async Task<PagedResult<StudentListItemResponse>> GetStudentsAsync(StudentFilterParams filters)
    {
        var query = _context.Students.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
        {
            var search = filters.SearchTerm.Trim().ToLower();
            query = query.Where(s => s.FullName.ToLower().Contains(search) || s.Email.ToLower().Contains(search));
        }

        if (filters.IsActive.HasValue)
        {
            query = query.Where(s => s.IsActive == filters.IsActive.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((filters.PageNumber - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .Select(s => new StudentListItemResponse
            {
                StudentId = s.StudentId,
                FullName = s.FullName,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
                IsActive = s.IsActive,
                ActiveEnrollmentsCount = s.Enrollments.Count(e => e.Status == Common.EnrollmentStatus.Active),
                CreatedAt = s.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<StudentListItemResponse>(items, totalCount, filters.PageNumber, filters.PageSize);
    }

    public async Task<StudentDetailsResponse> GetStudentByIdAsync(int id)
    {
        var student = await _context.Students
            .AsNoTracking()
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.TrainingTrack)
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Payments)
            .FirstOrDefaultAsync(s => s.StudentId == id);

        if (student == null)
        {
            throw new NotFoundException($"Student with ID {id} was not found.");
        }

        return new StudentDetailsResponse
        {
            StudentId = student.StudentId,
            FullName = student.FullName,
            Email = student.Email,
            PhoneNumber = student.PhoneNumber,
            IsActive = student.IsActive,
            CreatedAt = student.CreatedAt,
            UpdatedAt = student.UpdatedAt,
            Enrollments = student.Enrollments.Select(e => new StudentEnrollmentSummaryDto
            {
                EnrollmentId = e.EnrollmentId,
                TrainingTrackId = e.TrainingTrackId,
                TrackCode = e.TrainingTrack?.Code ?? string.Empty,
                TrackTitle = e.TrainingTrack?.Title ?? string.Empty,
                EnrollmentDate = e.EnrollmentDate,
                Status = e.Status,
                ProgressPercentage = e.ProgressPercentage,
                FinalResult = e.FinalResult,
                TrackPrice = e.TrainingTrack?.Price ?? 0m,
                TotalPaid = e.Payments.Where(p => p.PaymentStatus == Common.PaymentStatus.Completed).Sum(p => p.Amount)
            }).ToList()
        };
    }

    public async Task<StudentDetailsResponse> CreateStudentAsync(CreateStudentRequest request)
    {
        // Check unique email
        var emailExists = await _context.Students.AnyAsync(s => s.Email.ToLower() == request.Email.Trim().ToLower());
        if (emailExists)
        {
            throw new ConflictException($"A student with email '{request.Email}' already exists.");
        }

        var student = new Student
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim().ToLower(),
            PhoneNumber = request.PhoneNumber?.Trim(),
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        return await GetStudentByIdAsync(student.StudentId);
    }

    public async Task<StudentDetailsResponse> UpdateStudentAsync(int id, UpdateStudentRequest request)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == id);
        if (student == null)
        {
            throw new NotFoundException($"Student with ID {id} was not found.");
        }

        // Check unique email if email changed
        if (student.Email.ToLower() != request.Email.Trim().ToLower())
        {
            var emailExists = await _context.Students.AnyAsync(s => s.Email.ToLower() == request.Email.Trim().ToLower() && s.StudentId != id);
            if (emailExists)
            {
                throw new ConflictException($"A student with email '{request.Email}' already exists.");
            }
        }

        student.FullName = request.FullName.Trim();
        student.Email = request.Email.Trim().ToLower();
        student.PhoneNumber = request.PhoneNumber?.Trim();
        student.IsActive = request.IsActive;
        student.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetStudentByIdAsync(student.StudentId);
    }

    public async Task<bool> SoftDeleteStudentAsync(int id)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == id);
        if (student == null)
        {
            throw new NotFoundException($"Student with ID {id} was not found.");
        }

        student.IsDeleted = true;
        student.DeletedAt = DateTime.UtcNow;
        student.IsActive = false;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<StudentEnrollmentSummaryDto>> GetStudentEnrollmentsAsync(int studentId)
    {
        var studentExists = await _context.Students.AnyAsync(s => s.StudentId == studentId);
        if (!studentExists)
        {
            throw new NotFoundException($"Student with ID {studentId} was not found.");
        }

        return await _context.Enrollments
            .AsNoTracking()
            .Where(e => e.StudentId == studentId)
            .Include(e => e.TrainingTrack)
            .Include(e => e.Payments)
            .OrderByDescending(e => e.EnrollmentDate)
            .Select(e => new StudentEnrollmentSummaryDto
            {
                EnrollmentId = e.EnrollmentId,
                TrainingTrackId = e.TrainingTrackId,
                TrackCode = e.TrainingTrack != null ? e.TrainingTrack.Code : string.Empty,
                TrackTitle = e.TrainingTrack != null ? e.TrainingTrack.Title : string.Empty,
                EnrollmentDate = e.EnrollmentDate,
                Status = e.Status,
                ProgressPercentage = e.ProgressPercentage,
                FinalResult = e.FinalResult,
                TrackPrice = e.TrainingTrack != null ? e.TrainingTrack.Price : 0m,
                TotalPaid = e.Payments.Where(p => p.PaymentStatus == Common.PaymentStatus.Completed).Sum(p => p.Amount)
            })
            .ToListAsync();
    }
}
