using TrainingCenter.Api.Common;

namespace TrainingCenter.Api.Entities;

public class TrainingTrack : BaseAuditableEntity
{
    public int TrainingTrackId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationHours { get; set; }
    public int Capacity { get; set; } = 30;
    public TrackStatus Status { get; set; } = TrackStatus.Upcoming;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // Foreign Key: Primary Instructor (Required by Rule T4)
    public int? PrimaryInstructorId { get; set; }
    public virtual Instructor? PrimaryInstructor { get; set; }

    // Navigation properties
    private readonly List<Enrollment> _enrollments = new();
    public virtual IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    public void AddEnrollment(Enrollment enrollment)
    {
        _enrollments.Add(enrollment);
    }
}
