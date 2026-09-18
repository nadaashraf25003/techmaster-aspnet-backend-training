using TechMaster.Phase03.Task02.Common;
using TechMaster.Phase03.Task02.Entities.Bonus;

namespace TechMaster.Phase03.Task02.Entities;

public class Student : BaseSoftDeletableEntity
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    private readonly List<Enrollment> _enrollments = new();
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    // Bonus Navigations
    private readonly List<Attendance> _attendances = new();
    public IReadOnlyCollection<Attendance> Attendances => _attendances.AsReadOnly();

    private readonly List<AssignmentSubmission> _submissions = new();
    public IReadOnlyCollection<AssignmentSubmission> Submissions => _submissions.AsReadOnly();
}
