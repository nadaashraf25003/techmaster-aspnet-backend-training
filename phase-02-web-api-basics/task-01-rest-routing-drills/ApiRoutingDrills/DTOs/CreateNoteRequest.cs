using System.ComponentModel.DataAnnotations;

namespace ApiRoutingDrills.DTOs;

public class CreateNoteRequest
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 100 characters.")]
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;
}
