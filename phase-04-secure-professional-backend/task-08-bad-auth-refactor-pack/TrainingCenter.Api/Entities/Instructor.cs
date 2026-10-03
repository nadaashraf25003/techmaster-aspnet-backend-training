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
    public virtual ICollection<TrainingTrack> TrainingTracks { get; set; } = new List<TrainingTrack>();
}
