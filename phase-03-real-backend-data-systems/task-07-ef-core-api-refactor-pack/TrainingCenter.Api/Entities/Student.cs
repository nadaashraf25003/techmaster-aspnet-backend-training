using TrainingCenter.Api.Common;

namespace TrainingCenter.Api.Entities;

public class Student : BaseAuditableEntity
{
    private readonly List<Enrollment> _enrollments = new();

    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;

    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();
}
