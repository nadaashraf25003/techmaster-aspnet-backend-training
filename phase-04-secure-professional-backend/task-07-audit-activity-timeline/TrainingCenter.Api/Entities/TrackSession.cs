using TrainingCenter.Api.Common;

namespace TrainingCenter.Api.Entities;

public class TrackSession : BaseAuditableEntity
{
    public int TrackSessionId { get; set; }

    // Foreign Key: TrainingTrack
    public int TrainingTrackId { get; set; }
    public virtual TrainingTrack TrainingTrack { get; set; } = null!;

    public DateTime SessionDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MeetingLink { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public string? Notes { get; set; }

    // Foreign Key: Instructor who created / leads the session
    public int CreatedByInstructorId { get; set; }
    public virtual Instructor CreatedByInstructor { get; set; } = null!;
}
