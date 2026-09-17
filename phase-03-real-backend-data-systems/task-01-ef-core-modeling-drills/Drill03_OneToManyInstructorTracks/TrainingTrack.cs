using EFCoreModelingDrills.Common;
using EFCoreModelingDrills.Drill04_ManyToManyEnrollment;

namespace EFCoreModelingDrills.Drill03_OneToManyInstructorTracks;

/// <summary>
/// Drill 03: TrainingTrack Entity (Dependent entity in 1:N with Instructor, Principal in M:N with Students).
/// </summary>
public class TrainingTrack : BaseAuditableEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TrackLevel Level { get; set; } = TrackLevel.Beginner;
    public int Capacity { get; set; } = 25;
    public decimal Price { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // 1:N Foreign Key & Navigation to Instructor
    public int InstructorId { get; set; }
    public Instructor? Instructor { get; set; }

    // Drill 04: M:N Navigation to Enrollments (Encapsulated)
    private readonly List<Enrollment> _enrollments = new();
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    public void AddEnrollment(Enrollment enrollment)
    {
        ArgumentNullException.ThrowIfNull(enrollment);
        _enrollments.Add(enrollment);
    }
}
