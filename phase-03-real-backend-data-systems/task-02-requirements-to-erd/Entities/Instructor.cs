using TechMaster.Phase03.Task02.Common;

namespace TechMaster.Phase03.Task02.Entities;

public class Instructor : BaseAuditableEntity
{
    public int InstructorId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    private readonly List<TrainingTrack> _trainingTracks = new();
    public IReadOnlyCollection<TrainingTrack> TrainingTracks => _trainingTracks.AsReadOnly();
}
