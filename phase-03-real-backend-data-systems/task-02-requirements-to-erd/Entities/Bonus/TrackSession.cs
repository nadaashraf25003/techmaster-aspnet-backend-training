using TechMaster.Phase03.Task02.Common;

namespace TechMaster.Phase03.Task02.Entities.Bonus;

public class TrackSession
{
    public int SessionId { get; set; }

    public int TrainingTrackId { get; set; }
    public TrainingTrack? TrainingTrack { get; set; }

    public string Title { get; set; } = string.Empty;
    public DateOnly SessionDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string? RoomOrLink { get; set; }
    public bool IsCompleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    private readonly List<Attendance> _attendances = new();
    public IReadOnlyCollection<Attendance> Attendances => _attendances.AsReadOnly();
}
