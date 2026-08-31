using Microsoft.AspNetCore.Mvc;
using BookStoreApi.DTOs;
using BookStoreApi.Services;

namespace BookStoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;
    private readonly IBookService _bookService;
    private readonly ILogger<AuthorsController> _logger;

    public AuthorsController(
        IAuthorService authorService,
        IBookService bookService,
        ILogger<AuthorsController> logger)
    {
        _authorService = authorService;
        _bookService = bookService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AuthorResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var authors = (await _authorService.GetAllAuthorsAsync()).ToList();
        foreach (var author in authors)
        {
            author.BooksCount = await _bookService.GetBookCountForAuthorAsync(author.AuthorId);
        }

        return Ok(authors);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AuthorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var author = await _authorService.GetAuthorByIdAsync(id);
        if (author == null)
        {
            return NotFound(new ApiErrorResponse { Message = $"Author with ID {id} was not found." });
        }

        author.BooksCount = await _bookService.GetBookCountForAuthorAsync(id);
        return Ok(author);
    }


    [HttpPost]
    [ProducesResponseType(typeof(AuthorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateAuthorRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, error, data) = await _authorService.CreateAuthorAsync(request);
        if (!success)
        {
            return BadRequest(new ApiErrorResponse { Message = error ?? "Failed to create author." });
        }

        return CreatedAtAction(nameof(GetById), new { id = data!.AuthorId }, data);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(AuthorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateAuthorRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, error, data) = await _authorService.UpdateAuthorAsync(id, request);
        if (!success)
        {
            return NotFound(new ApiErrorResponse { Message = error ?? $"Author with ID {id} was not found." });
        }

        data!.BooksCount = await _bookService.GetBookCountForAuthorAsync(id);
        return Ok(data);
    }

    
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var author = await _authorService.GetAuthorByIdAsync(id);
        if (author == null)
        {
            return NotFound(new ApiErrorResponse { Message = $"Author with ID {id} was not found." });
        }

        // Business rule: Check if author has associated books
        var hasBooks = await _bookService.HasBooksForAuthorAsync(id);
        if (hasBooks)
        {
            return BadRequest(new ApiErrorResponse
            {
                Message = $"Cannot delete author '{author.FullName}' (ID {id}) because they have associated books in the store. Please delete or reassign the books first."
            });
        }

        var (success, error) = await _authorService.DeleteAuthorAsync(id);
        if (!success)
        {
            return BadRequest(new ApiErrorResponse { Message = error ?? "Failed to delete author." });
        }

        return Ok(new ApiResponse
        {
            Success = true,
            Message = $"Author '{author.FullName}' (ID {id}) was successfully deleted."
        });
    }
}
