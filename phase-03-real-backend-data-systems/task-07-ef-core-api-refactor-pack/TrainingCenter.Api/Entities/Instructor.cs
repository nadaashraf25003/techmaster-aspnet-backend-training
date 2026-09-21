using TrainingCenter.Api.Common;

namespace TrainingCenter.Api.Entities;

public class Instructor : BaseAuditableEntity
{
    private readonly List<TrainingTrack> _trainingTracks = new();

    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public bool IsActive { get; set; } = true;

    public IReadOnlyCollection<TrainingTrack> TrainingTracks => _trainingTracks.AsReadOnly();
}
