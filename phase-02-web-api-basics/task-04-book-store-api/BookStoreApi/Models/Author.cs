namespace BookStoreApi.Models;
public class Author
{
    public int AuthorId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
