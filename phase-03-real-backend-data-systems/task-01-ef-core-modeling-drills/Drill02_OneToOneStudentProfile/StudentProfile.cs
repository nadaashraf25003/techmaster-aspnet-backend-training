using EFCoreModelingDrills.Drill01_DbContextFirstMigration;

namespace EFCoreModelingDrills.Drill02_OneToOneStudentProfile;

/// <summary>
/// Drill 02: One-to-One StudentProfile Entity.
/// Maps 1:1 relationship with Student through unique Foreign Key (StudentId).
/// </summary>
public class StudentProfile
{
    public int Id { get; set; }
    public string NationalId { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string EmergencyPhone { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }

    // Foreign Key & Navigation Property (Dependent entity)
    public int StudentId { get; set; }
    public Student? Student { get; set; }
}
