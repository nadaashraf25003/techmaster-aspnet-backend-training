namespace TechMaster.Phase03.Task02.Entities.Bonus;

public class AssignmentSubmission
{
    public int SubmissionId { get; set; }

    public int AssignmentId { get; set; }
    public Assignment? Assignment { get; set; }

    public int StudentId { get; set; }
    public Student? Student { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public string ContentUrl { get; set; } = string.Empty;
    public decimal? Score { get; set; }
    public string? Feedback { get; set; }
    public DateTime? GradedAt { get; set; }
}
