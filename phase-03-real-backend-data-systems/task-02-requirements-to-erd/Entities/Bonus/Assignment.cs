namespace TechMaster.Phase03.Task02.Entities.Bonus;

public class Assignment
{
    public int AssignmentId { get; set; }

    public int TrainingTrackId { get; set; }
    public TrainingTrack? TrainingTrack { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public decimal MaxScore { get; set; } = 100.00m;
    public decimal WeightPercentage { get; set; } = 10.00m;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    private readonly List<AssignmentSubmission> _submissions = new();
    public IReadOnlyCollection<AssignmentSubmission> Submissions => _submissions.AsReadOnly();
}
