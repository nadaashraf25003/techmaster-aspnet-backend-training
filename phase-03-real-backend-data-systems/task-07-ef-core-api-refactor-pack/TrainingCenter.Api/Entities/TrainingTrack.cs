using TrainingCenter.Api.Common;

namespace TrainingCenter.Api.Entities;

public class TrainingTrack : BaseAuditableEntity
{
    private readonly List<Enrollment> _enrollments = new();

    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Capacity { get; set; }
    public TrackStatus Status { get; set; } = TrackStatus.Planned;
    public int? InstructorId { get; set; }
    public Instructor? Instructor { get; set; }

    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();
}
