using StudentManagementApi.DTOs;
using StudentManagementApi.Models;

namespace StudentManagementApi.Services;

/// <summary>
/// In-memory implementation of the student service with pre-seeded data.
/// </summary>
public class StudentService : IStudentService
{
    private readonly List<Student> _students = new();
    private readonly Lock _lock = new();
    private int _nextId = 1;

    public StudentService()
    {
        SeedInitialData();
    }

    private void SeedInitialData()
    {
        var seedList = new List<Student>
        {
            new()
            {
                Id = _nextId++,
                FullName = "Ahmed Hassan",
                Email = "ahmed.hassan@techmaster.com",
                PhoneNumber = "+201012345678",
                TrackName = "ASP.NET Backend",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddMonths(-3)
            },
            new()
            {
                Id = _nextId++,
                FullName = "Sara Mahmoud",
                Email = "sara.mahmoud@techmaster.com",
                PhoneNumber = "+201023456789",
                TrackName = "ASP.NET Backend",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddMonths(-2)
            },
            new()
            {
                Id = _nextId++,
                FullName = "Omar Khaled",
                Email = "omar.khaled@techmaster.com",
                PhoneNumber = "+201034567890",
                TrackName = "Frontend React",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddMonths(-2)
            },
            new()
            {
                Id = _nextId++,
                FullName = "Nourhan Ali",
                Email = "nourhan.ali@techmaster.com",
                PhoneNumber = "+201045678901",
                TrackName = "Frontend React",
                IsActive = false,
                CreatedAt = DateTime.UtcNow.AddMonths(-4),
                UpdatedAt = DateTime.UtcNow.AddMonths(-1)
            },
            new()
            {
                Id = _nextId++,
                FullName = "Youssef Ibrahim",
                Email = "youssef.ibrahim@techmaster.com",
                PhoneNumber = "+201056789012",
                TrackName = "Mobile Flutter",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddMonths(-1)
            },
            new()
            {
                Id = _nextId++,
                FullName = "Mariam Tarek",
                Email = "mariam.tarek@techmaster.com",
                PhoneNumber = "+201067890123",
                TrackName = "Mobile Flutter",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            },
            new()
            {
                Id = _nextId++,
                FullName = "Karim Adel",
                Email = "karim.adel@techmaster.com",
                PhoneNumber = "+201078901234",
                TrackName = "DevOps & Cloud",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            },
            new()
            {
                Id = _nextId++,
                FullName = "Salma Farouk",
                Email = "salma.farouk@techmaster.com",
                PhoneNumber = "+201089012345",
                TrackName = "DevOps & Cloud",
                IsActive = false,
                CreatedAt = DateTime.UtcNow.AddMonths(-3),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            }
        };

        _students.AddRange(seedList);
    }

    public Task<PagedResponse<StudentResponseDto>> GetAllStudentsAsync(StudentQueryParametersDto query)
    {
        lock (_lock)
        {
            IEnumerable<Student> filtered = _students;

            // 1. Search by Name or Email
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim();
                filtered = filtered.Where(s =>
                    s.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    s.Email.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            // 2. Filter by TrackName
            if (!string.IsNullOrWhiteSpace(query.TrackName))
            {
                var track = query.TrackName.Trim();
                filtered = filtered.Where(s =>
                    s.TrackName.Equals(track, StringComparison.OrdinalIgnoreCase));
            }

            // 3. Filter by IsActive
            if (query.IsActive.HasValue)
            {
                filtered = filtered.Where(s => s.IsActive == query.IsActive.Value);
            }

            var filteredList = filtered.OrderBy(s => s.Id).ToList();
            var totalCount = filteredList.Count;

            // 4. Pagination
            var pagedItems = filteredList
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(MapToDto)
                .ToList();

            var response = new PagedResponse<StudentResponseDto>(
                pagedItems,
                totalCount,
                query.PageNumber,
                query.PageSize
            );

            return Task.FromResult(response);
        }
    }

    public Task<StudentResponseDto?> GetStudentByIdAsync(int id)
    {
        lock (_lock)
        {
            var student = _students.FirstOrDefault(s => s.Id == id);
            return Task.FromResult(student != null ? MapToDto(student) : null);
        }
    }

    public Task<(bool Success, string? ErrorMessage, StudentResponseDto? Student)> CreateStudentAsync(CreateStudentDto dto)
    {
        lock (_lock)
        {
            // Business Rule: Email uniqueness
            var emailNormalized = dto.Email.Trim();
            if (_students.Any(s => s.Email.Equals(emailNormalized, StringComparison.OrdinalIgnoreCase)))
            {
                return Task.FromResult<(bool, string?, StudentResponseDto?)>((
                    false,
                    $"A student with the email '{dto.Email}' already exists in the system.",
                    null
                ));
            }

            var newStudent = new Student
            {
                Id = _nextId++,
                FullName = dto.FullName.Trim(),
                Email = emailNormalized,
                PhoneNumber = dto.PhoneNumber.Trim(),
                TrackName = dto.TrackName.Trim(),
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            _students.Add(newStudent);
            return Task.FromResult<(bool, string?, StudentResponseDto?)>((true, null, MapToDto(newStudent)));
        }
    }

    public Task<(bool Success, string? ErrorMessage, StudentResponseDto? Student)> UpdateStudentAsync(int id, UpdateStudentDto dto)
    {
        lock (_lock)
        {
            var student = _students.FirstOrDefault(s => s.Id == id);
            if (student == null)
            {
                return Task.FromResult<(bool, string?, StudentResponseDto?)>((
                    false,
                    $"Student with ID {id} was not found.",
                    null
                ));
            }

            var emailNormalized = dto.Email.Trim();
            // Check if another student has this email
            if (_students.Any(s => s.Id != id && s.Email.Equals(emailNormalized, StringComparison.OrdinalIgnoreCase)))
            {
                return Task.FromResult<(bool, string?, StudentResponseDto?)>((
                    false,
                    $"Another student with the email '{dto.Email}' already exists in the system.",
                    null
                ));
            }

            // StudentId cannot change
            student.FullName = dto.FullName.Trim();
            student.Email = emailNormalized;
            student.PhoneNumber = dto.PhoneNumber.Trim();
            student.TrackName = dto.TrackName.Trim();
            student.IsActive = dto.IsActive;
            student.UpdatedAt = DateTime.UtcNow;

            return Task.FromResult<(bool, string?, StudentResponseDto?)>((true, null, MapToDto(student)));
        }
    }

    public Task<(bool Success, string? ErrorMessage, StudentResponseDto? Student)> UpdateStudentStatusAsync(int id, bool isActive)
    {
        lock (_lock)
        {
            var student = _students.FirstOrDefault(s => s.Id == id);
            if (student == null)
            {
                return Task.FromResult<(bool, string?, StudentResponseDto?)>((
                    false,
                    $"Student with ID {id} was not found.",
                    null
                ));
            }

            student.IsActive = isActive;
            student.UpdatedAt = DateTime.UtcNow;

            return Task.FromResult<(bool, string?, StudentResponseDto?)>((true, null, MapToDto(student)));
        }
    }

    public Task<StudentStatsDto> GetStudentStatsAsync()
    {
        lock (_lock)
        {
            var total = _students.Count;
            var active = _students.Count(s => s.IsActive);
            var inactive = _students.Count(s => !s.IsActive);

            var trackCounts = _students
                .GroupBy(s => s.TrackName, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.Count());

            var stats = new StudentStatsDto
            {
                TotalStudents = total,
                ActiveStudents = active,
                InactiveStudents = inactive,
                CountByTrack = trackCounts
            };

            return Task.FromResult(stats);
        }
    }

    private static StudentResponseDto MapToDto(Student student)
    {
        return new StudentResponseDto
        {
            Id = student.Id,
            FullName = student.FullName,
            Email = student.Email,
            PhoneNumber = student.PhoneNumber,
            TrackName = student.TrackName,
            IsActive = student.IsActive,
            CreatedAt = student.CreatedAt,
            UpdatedAt = student.UpdatedAt
        };
    }
}
