using TechMaster.Phase03.Task02.Common;

namespace TechMaster.Phase03.Task02.Entities.Bonus;

public class Attendance
{
    public int AttendanceId { get; set; }

    public int TrackSessionId { get; set; }
    public TrackSession? TrackSession { get; set; }

    public int StudentId { get; set; }
    public Student? Student { get; set; }

    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
    public DateTime MarkedAt { get; set; } = DateTime.UtcNow;
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
