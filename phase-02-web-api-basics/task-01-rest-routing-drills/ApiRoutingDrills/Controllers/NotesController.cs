using ApiRoutingDrills.DTOs;
using ApiRoutingDrills.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiRoutingDrills.Controllers;

/// <summary>
/// Drills 06 to 12: In-Memory Notes CRUD, Search & Pagination
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private static readonly List<Note> _notes = new()
    {
        new Note { Id = 1, Title = "Getting Started with ASP.NET Core", Content = "Learn the basics of controllers, actions, and dependency injection.", CreatedAt = DateTime.UtcNow.AddDays(-5) },
        new Note { Id = 2, Title = "Understanding REST Principles", Content = "RESTful APIs use HTTP verbs (GET, POST, PUT, DELETE) and standard status codes.", CreatedAt = DateTime.UtcNow.AddDays(-4) },
        new Note { Id = 3, Title = "DTOs vs Models", Content = "Always use DTOs to encapsulate request and response contracts.", CreatedAt = DateTime.UtcNow.AddDays(-3) },
        new Note { Id = 4, Title = "Swagger and OpenAPI", Content = "Document your endpoints automatically using Swashbuckle / OpenAPI tools.", CreatedAt = DateTime.UtcNow.AddDays(-2) },
        new Note { Id = 5, Title = "Pagination Best Practices", Content = "Use pageNumber and pageSize with Skip and Take for efficient data paging.", CreatedAt = DateTime.UtcNow.AddDays(-1) }
    };

    private static readonly object _lock = new();

    /// <summary>
    /// Drill 07: Get all notes
    /// </summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        lock (_lock)
        {
            return Ok(_notes.ToList());
        }
    }

    /// <summary>
    /// Drill 08: Get note by Id
    /// </summary>
    [HttpGet("{id:int}")]
    public IActionResult GetById([FromRoute] int id)
    {
        lock (_lock)
        {
            var note = _notes.FirstOrDefault(n => n.Id == id);
            if (note == null)
            {
                return NotFound(new { message = $"Note with id {id} not found." });
            }

            return Ok(note);
        }
    }

    /// <summary>
    /// Drill 06: Create note with request body DTO
    /// </summary>
    [HttpPost]
    public IActionResult Create([FromBody] CreateNoteRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Note newNote;
        lock (_lock)
        {
            int nextId = _notes.Count > 0 ? _notes.Max(n => n.Id) + 1 : 1;
            newNote = new Note
            {
                Id = nextId,
                Title = request.Title.Trim(),
                Content = request.Content?.Trim() ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            _notes.Add(newNote);
        }

        return CreatedAtAction(nameof(GetById), new { id = newNote.Id }, newNote);
    }

    /// <summary>
    /// Drill 09: Update note
    /// </summary>
    [HttpPut("{id:int}")]
    public IActionResult Update([FromRoute] int id, [FromBody] UpdateNoteRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        lock (_lock)
        {
            var existingNote = _notes.FirstOrDefault(n => n.Id == id);
            if (existingNote == null)
            {
                return NotFound(new { message = $"Note with id {id} not found." });
            }

            existingNote.Title = request.Title.Trim();
            existingNote.Content = request.Content.Trim();
            existingNote.UpdatedAt = DateTime.UtcNow;

            return Ok(existingNote);
        }
    }

    /// <summary>
    /// Drill 10: Delete note
    /// </summary>
    [HttpDelete("{id:int}")]
    public IActionResult Delete([FromRoute] int id)
    {
        lock (_lock)
        {
            var existingNote = _notes.FirstOrDefault(n => n.Id == id);
            if (existingNote == null)
            {
                return NotFound(new { message = $"Note with id {id} not found." });
            }

            _notes.Remove(existingNote);
            return NoContent();
        }
    }

    /// <summary>
    /// Drill 11: Search notes by keyword in Title or Content
    /// </summary>
    [HttpGet("search")]
    public IActionResult Search([FromQuery] string? keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return BadRequest(new { error = "Search keyword query parameter is required." });
        }

        lock (_lock)
        {
            var results = _notes
                .Where(n => n.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                            n.Content.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Ok(results);
        }
    }

    /// <summary>
    /// Drill 12: Paginated notes listing
    /// </summary>
    [HttpGet("paged")]
    public IActionResult GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
    {
        if (pageNumber < 1)
        {
            return BadRequest(new { error = "pageNumber must be greater than or equal to 1." });
        }

        if (pageSize < 1 || pageSize > 50)
        {
            return BadRequest(new { error = "pageSize must be between 1 and 50." });
        }

        lock (_lock)
        {
            int totalCount = _notes.Count;
            var items = _notes
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new PagedResult<Note>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Ok(response);
        }
    }
}
