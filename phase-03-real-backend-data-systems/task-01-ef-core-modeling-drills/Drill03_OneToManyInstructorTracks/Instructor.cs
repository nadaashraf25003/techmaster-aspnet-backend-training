using EFCoreModelingDrills.Common;

namespace EFCoreModelingDrills.Drill03_OneToManyInstructorTracks;

/// <summary>
/// Drill 03: Instructor Entity (Principal entity in 1:N with TrainingTracks).
/// Encapsulates tracks collection with IReadOnlyCollection.
/// </summary>
public class Instructor : BaseAuditableEntity
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public bool IsActive { get; set; } = true;

    // Encapsulated 1:N Navigation Property
    private readonly List<TrainingTrack> _tracks = new();
    public IReadOnlyCollection<TrainingTrack> Tracks => _tracks.AsReadOnly();

    public void AssignTrack(TrainingTrack track)
    {
        ArgumentNullException.ThrowIfNull(track);
        _tracks.Add(track);
    }
}
