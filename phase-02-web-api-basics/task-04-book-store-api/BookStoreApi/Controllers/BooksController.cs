using Microsoft.AspNetCore.Mvc;
using BookStoreApi.DTOs;
using BookStoreApi.Services;

namespace BookStoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly ILogger<BooksController> _logger;

    public BooksController(IBookService bookService, ILogger<BooksController> logger)
    {
        _bookService = bookService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<BookResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] BookFilterQuery query)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _bookService.GetPagedBooksAsync(query);
        return Ok(result);
    }


    [HttpGet("reports/summary")]
    [ProducesResponseType(typeof(BookStoreReportResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReportSummary()
    {
        var report = await _bookService.GetReportSummaryAsync();
        return Ok(report);
    }


    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BookResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
        {
            return NotFound(new ApiErrorResponse { Message = $"Book with ID {id} was not found." });
        }

        return Ok(book);
    }


    [HttpPost]
    [ProducesResponseType(typeof(BookResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateBookRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, error, data) = await _bookService.CreateBookAsync(request);
        if (!success)
        {
            return BadRequest(new ApiErrorResponse { Message = error ?? "Failed to create book." });
        }

        return CreatedAtAction(nameof(GetById), new { id = data!.BookId }, data);
    }


    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(BookResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateBookRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, error, data) = await _bookService.UpdateBookAsync(id, request);
        if (!success)
        {
            if (error != null && error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new ApiErrorResponse { Message = error });
            }

            return BadRequest(new ApiErrorResponse { Message = error ?? "Failed to update book." });
        }

        return Ok(data);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var (success, error) = await _bookService.DeleteBookAsync(id);
        if (!success)
        {
            return NotFound(new ApiErrorResponse { Message = error ?? $"Book with ID {id} was not found." });
        }

        return Ok(new ApiResponse
        {
            Success = true,
            Message = $"Book with ID {id} was successfully deleted."
        });
    }
}
