using TechMaster.Phase03.Task02.Common;
using TechMaster.Phase03.Task02.Entities.Bonus;

namespace TechMaster.Phase03.Task02.Entities;

public class TrainingTrack : BaseSoftDeletableEntity
{
    public int TrainingTrackId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TrackLevel Level { get; set; }
    public decimal Price { get; set; }
    public int Capacity { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public TrackStatus Status { get; set; } = TrackStatus.Draft;

    // Foreign Key
    public int InstructorId { get; set; }
    public Instructor? Instructor { get; set; }

    // Navigation Properties
    private readonly List<Enrollment> _enrollments = new();
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    // Bonus Navigations
    private readonly List<TrackSession> _sessions = new();
    public IReadOnlyCollection<TrackSession> Sessions => _sessions.AsReadOnly();

    private readonly List<Assignment> _assignments = new();
    public IReadOnlyCollection<Assignment> Assignments => _assignments.AsReadOnly();
}
