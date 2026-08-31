using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.DTOs;

public class CreateAuthorRequest
{
    [Required(ErrorMessage = "Author Full Name is required.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Author Full Name must be between 2 and 150 characters.")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Bio cannot exceed 1000 characters.")]
    public string? Bio { get; set; }
}

public class UpdateAuthorRequest
{
    [Required(ErrorMessage = "Author Full Name is required.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Author Full Name must be between 2 and 150 characters.")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Bio cannot exceed 1000 characters.")]
    public string? Bio { get; set; }
}

public class AuthorResponse
{
    public int AuthorId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; }
    public int BooksCount { get; set; }
}
