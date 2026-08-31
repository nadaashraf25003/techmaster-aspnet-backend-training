using Microsoft.AspNetCore.Mvc;
using BookStoreApi.DTOs;
using BookStoreApi.Services;

namespace BookStoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly IBookService _bookService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(
        ICategoryService categoryService,
        IBookService bookService,
        ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _bookService = bookService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
    {
        var categories = (await _categoryService.GetAllCategoriesAsync(includeInactive)).ToList();
        foreach (var category in categories)
        {
            category.BooksCount = await _bookService.GetBookCountForCategoryAsync(category.CategoryId);
        }

        return Ok(categories);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound(new ApiErrorResponse { Message = $"Category with ID {id} was not found." });
        }

        category.BooksCount = await _bookService.GetBookCountForCategoryAsync(id);
        return Ok(category);
    }


    [HttpPost]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, error, data) = await _categoryService.CreateCategoryAsync(request);
        if (!success)
        {
            return BadRequest(new ApiErrorResponse { Message = error ?? "Failed to create category." });
        }

        return CreatedAtAction(nameof(GetById), new { id = data!.CategoryId }, data);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCategoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, error, data) = await _categoryService.UpdateCategoryAsync(id, request);
        if (!success)
        {
            if (error != null && error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new ApiErrorResponse { Message = error });
            }
            return BadRequest(new ApiErrorResponse { Message = error ?? "Failed to update category." });
        }

        data!.BooksCount = await _bookService.GetBookCountForCategoryAsync(id);
        return Ok(data);
    }


    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound(new ApiErrorResponse { Message = $"Category with ID {id} was not found." });
        }

        // Business rule: Check if category contains books
        var hasBooks = await _bookService.HasBooksForCategoryAsync(id);
        if (hasBooks)
        {
            return BadRequest(new ApiErrorResponse
            {
                Message = $"Cannot delete category '{category.Name}' (ID {id}) because it has associated books in the store. Please delete or reassign the books first."
            });
        }

        var (success, error) = await _categoryService.DeleteCategoryAsync(id);
        if (!success)
        {
            return BadRequest(new ApiErrorResponse { Message = error ?? "Failed to delete category." });
        }

        return Ok(new ApiResponse
        {
            Success = true,
            Message = $"Category '{category.Name}' (ID {id}) was successfully deleted."
        });
    }
}
