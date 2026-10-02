using TrainingCenter.Api.Common;

namespace TrainingCenter.Api.Entities;

public class Student : BaseAuditableEntity
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    private readonly List<Enrollment> _enrollments = new();
    public virtual IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    public void AddEnrollment(Enrollment enrollment)
    {
        _enrollments.Add(enrollment);
    }
}
