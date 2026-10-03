using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common;
using TrainingCenter.Api.Common.Exceptions;
using TrainingCenter.Api.Data;
using TrainingCenter.Api.DTOs.Common;
using TrainingCenter.Api.DTOs.Enrollments;
using TrainingCenter.Api.DTOs.Payments;
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

    /// <summary>
    /// Rule S1: Retrieves a single student by primary key.
    /// </summary>
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
    /// Rule S2: Student full name, email, and valid DateOfBirth are required. Email must be unique.
    /// </summary>
    public async Task<StudentResponse> CreateStudentAsync(CreateStudentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            throw new BadRequestException("Student FullName is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new BadRequestException("Student Email is required.");
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

        _context.Students.Add(student);
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

    public async Task<List<EnrollmentListItemResponse>> GetStudentEnrollmentsAsync(int studentId)
    {
        var studentExists = await _context.Students.AnyAsync(s => s.StudentId == studentId);
        if (!studentExists)
        {
            throw new NotFoundException($"Student with ID {studentId} was not found.");
        }

        var enrollments = await _context.Enrollments
            .AsNoTracking()
            .Include(e => e.TrainingTrack)
            .Include(e => e.Payments)
            .Include(e => e.Student)
            .Where(e => e.StudentId == studentId && !e.IsDeleted)
            .OrderByDescending(e => e.EnrollmentDate)
            .Select(e => new EnrollmentListItemResponse
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                StudentName = e.Student != null ? e.Student.FullName : string.Empty,
                StudentEmail = e.Student != null ? e.Student.Email : string.Empty,
                TrainingTrackId = e.TrainingTrackId,
                TrackCode = e.TrainingTrack != null ? e.TrainingTrack.Code : string.Empty,
                TrackTitle = e.TrainingTrack != null ? e.TrainingTrack.Title : string.Empty,
                TrackPrice = e.TrainingTrack != null ? e.TrainingTrack.Price : 0m,
                EnrollmentDate = e.EnrollmentDate,
                Status = e.Status,
                ProgressPercentage = e.ProgressPercentage,
                FinalResult = e.FinalResult,
                TotalPaid = e.Payments.Where(p => p.PaymentStatus == Common.PaymentStatus.Completed).Sum(p => (decimal?)p.Amount) ?? 0m
            })
            .ToListAsync();

        return enrollments;
    }

    public async Task<List<PaymentResponse>> GetStudentPaymentsAsync(int studentId)
    {
        var studentExists = await _context.Students.AnyAsync(s => s.StudentId == studentId);
        if (!studentExists)
        {
            throw new NotFoundException($"Student with ID {studentId} was not found.");
        }

        var payments = await _context.Payments
            .AsNoTracking()
            .Include(p => p.Enrollment)
            .ThenInclude(e => e!.TrainingTrack)
            .Include(p => p.Enrollment)
            .ThenInclude(e => e!.Student)
            .Where(p => p.Enrollment != null && p.Enrollment.StudentId == studentId && !p.IsDeleted)
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => new PaymentResponse
            {
                PaymentId = p.PaymentId,
                EnrollmentId = p.EnrollmentId,
                StudentName = p.Enrollment != null && p.Enrollment.Student != null ? p.Enrollment.Student.FullName : string.Empty,
                TrackTitle = p.Enrollment != null && p.Enrollment.TrainingTrack != null ? p.Enrollment.TrainingTrack.Title : string.Empty,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                PaymentDate = p.PaymentDate,
                PaymentStatus = p.PaymentStatus,
                ReferenceNumber = p.ReferenceNumber,
                Notes = p.Notes,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        return payments;
    }

    public async Task<StudentResponse> UpdateStudentProfileAsync(int studentId, UpdateStudentProfileRequest request)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
        if (student == null)
        {
            throw new NotFoundException($"Student profile for ID {studentId} was not found.");
        }

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            throw new BadRequestException("FullName is required.");
        }

        student.FullName = request.FullName.Trim();
        student.PhoneNumber = request.PhoneNumber?.Trim();
        if (request.DateOfBirth.HasValue)
        {
            student.DateOfBirth = request.DateOfBirth.Value;
        }
        student.Address = request.Address?.Trim();

        await _context.SaveChangesAsync();
        return await GetStudentByIdAsync(studentId);
    }

    public async Task<EnrollmentDetailsResponse> RequestEnrollmentAsync(int studentId, StudentEnrollmentRequest request)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
        if (student == null)
        {
            throw new NotFoundException($"Student with ID {studentId} was not found.");
        }

        if (!student.IsActive)
        {
            throw new BadRequestException("Inactive student account cannot request track enrollment.");
        }

        var track = await _context.TrainingTracks
            .Include(t => t.Enrollments)
            .Include(t => t.PrimaryInstructor)
            .FirstOrDefaultAsync(t => t.TrainingTrackId == request.TrainingTrackId);

        if (track == null)
        {
            throw new NotFoundException($"Training track with ID {request.TrainingTrackId} was not found.");
        }

        if (track.Status == TrackStatus.Closed || track.Status == TrackStatus.Archived)
        {
            throw new BadRequestException($"Cannot enroll in track '{track.Title}' because its status is {track.Status}.");
        }

        // Rule: Cannot enroll twice in the same active/pending track
        var existingEnrollment = await _context.Enrollments
            .AnyAsync(e => e.StudentId == studentId && 
                           e.TrainingTrackId == request.TrainingTrackId && 
                           (e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Pending));

        if (existingEnrollment)
        {
            throw new BadRequestException("Student is already enrolled or has a pending enrollment request for this track.");
        }

        // Rule: Check capacity
        var activeOrPendingCount = await _context.Enrollments
            .CountAsync(e => e.TrainingTrackId == request.TrainingTrackId && 
                             (e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Pending));

        if (activeOrPendingCount >= track.Capacity)
        {
            throw new BadRequestException($"Track '{track.Title}' has reached its maximum capacity ({track.Capacity} seats).");
        }

        var enrollment = new Enrollment
        {
            StudentId = studentId,
            TrainingTrackId = request.TrainingTrackId,
            EnrollmentDate = DateTime.UtcNow,
            Status = EnrollmentStatus.Pending,
            ProgressPercentage = 0.00m,
            Notes = request.Notes?.Trim()
        };

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        return new EnrollmentDetailsResponse
        {
            EnrollmentId = enrollment.EnrollmentId,
            StudentId = student.StudentId,
            StudentName = student.FullName,
            StudentEmail = student.Email,
            StudentPhone = student.PhoneNumber,
            TrainingTrackId = track.TrainingTrackId,
            TrackCode = track.Code,
            TrackTitle = track.Title,
            TrackPrice = track.Price,
            TrackStartDate = track.StartDate,
            TrackEndDate = track.EndDate,
            InstructorName = track.PrimaryInstructor != null ? track.PrimaryInstructor.FullName : null,
            EnrollmentDate = enrollment.EnrollmentDate,
            Status = enrollment.Status,
            ProgressPercentage = enrollment.ProgressPercentage,
            FinalResult = enrollment.FinalResult,
            Notes = enrollment.Notes,
            TotalPaid = 0m,
            Payments = new List<EnrollmentPaymentSummaryResponse>()
        };
    }
}
