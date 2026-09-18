using TrainingCenter.Api.Common;

namespace TrainingCenter.Api.Entities;

public class Instructor : BaseAuditableEntity
{
    public int InstructorId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    private readonly List<TrainingTrack> _trainingTracks = new();
    public virtual IReadOnlyCollection<TrainingTrack> TrainingTracks => _trainingTracks.AsReadOnly();

    public void AddTrainingTrack(TrainingTrack track)
    {
        _trainingTracks.Add(track);
    }
}
