using System.ComponentModel.DataAnnotations;

namespace TrainingCenter.Api.DTOs.Sessions;

public class CreateTrackSessionRequest
{
    [Required(ErrorMessage = "SessionDate is required.")]
    public DateTime SessionDate { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string Description { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "MeetingLink cannot exceed 500 characters.")]
    public string MeetingLink { get; set; } = string.Empty;

    [MaxLength(2000, ErrorMessage = "Notes cannot exceed 2000 characters.")]
    public string? Notes { get; set; }
}

public class UpdateTrackSessionRequest
{
    [Required(ErrorMessage = "SessionDate is required.")]
    public DateTime SessionDate { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string Description { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "MeetingLink cannot exceed 500 characters.")]
    public string MeetingLink { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    [MaxLength(2000, ErrorMessage = "Notes cannot exceed 2000 characters.")]
    public string? Notes { get; set; }
}

public class TrackSessionResponse
{
    public int TrackSessionId { get; set; }
    public int TrainingTrackId { get; set; }
    public string TrackTitle { get; set; } = string.Empty;
    public DateTime SessionDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MeetingLink { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public string? Notes { get; set; }
    public int CreatedByInstructorId { get; set; }
    public string CreatedByInstructorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
