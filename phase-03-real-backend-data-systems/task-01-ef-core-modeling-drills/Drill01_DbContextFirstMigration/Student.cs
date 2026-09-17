using EFCoreModelingDrills.Common;
using EFCoreModelingDrills.Drill02_OneToOneStudentProfile;
using EFCoreModelingDrills.Drill04_ManyToManyEnrollment;

namespace EFCoreModelingDrills.Drill01_DbContextFirstMigration;

/// <summary>
/// Drill 01: Core Student Entity.
/// Demonstrates basic EF Core mapping, audit timestamps, and collection encapsulation.
/// </summary>
public class Student : BaseAuditableEntity
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;

    // Drill 02: 1:1 Navigation Property
    public StudentProfile? Profile { get; set; }

    // Drill 04: 1:N Navigation Property to Join Entity (Encapsulated)
    private readonly List<Enrollment> _enrollments = new();
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    public void AddEnrollment(Enrollment enrollment)
    {
        ArgumentNullException.ThrowIfNull(enrollment);
        _enrollments.Add(enrollment);
    }
}
