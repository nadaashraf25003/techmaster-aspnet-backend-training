using Microsoft.AspNetCore.Mvc;
using ProductsCategoriesApi.DTOs;
using ProductsCategoriesApi.Services;

namespace ProductsCategoriesApi.Controllers;

/// <summary>
/// API Controller for managing product categories.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(
        ICategoryService categoryService,
        IProductService productService,
        ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// Feature 01: Return all product categories.
    /// Inactive categories are hidden by default.
    /// </summary>
    /// <param name="includeInactive">Whether to include inactive categories.</param>
    /// <returns>List of categories.</returns>
    /// <response code="200">Categories retrieved successfully.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
    {
        var categories = (await _categoryService.GetAllCategoriesAsync(includeInactive)).ToList();
        foreach (var cat in categories)
        {
            cat.ProductsCount = await _productService.GetProductCountForCategoryAsync(cat.CategoryId);
        }

        return Ok(categories);
    }

    /// <summary>
    /// Feature 01: Get category details by ID.
    /// </summary>
    /// <param name="id">Category unique ID.</param>
    /// <returns>Category details.</returns>
    /// <response code="200">Category found.</response>
    /// <response code="404">Category not found.</response>
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

        category.ProductsCount = await _productService.GetProductCountForCategoryAsync(id);
        return Ok(category);
    }

    /// <summary>
    /// Feature 01: Create a new category.
    /// </summary>
    /// <param name="request">Category creation payload.</param>
    /// <returns>The created category.</returns>
    /// <response code="201">Category created successfully.</response>
    /// <response code="400">Invalid payload or duplicate category name.</response>
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

    /// <summary>
    /// Feature 01: Update an existing category.
    /// </summary>
    /// <param name="id">Category ID.</param>
    /// <param name="request">Updated category payload.</param>
    /// <returns>The updated category.</returns>
    /// <response code="200">Category updated successfully.</response>
    /// <response code="400">Invalid payload or duplicate name.</response>
    /// <response code="404">Category not found.</response>
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

        data!.ProductsCount = await _productService.GetProductCountForCategoryAsync(id);
        return Ok(data);
    }

    /// <summary>
    /// Feature 01: Delete a category (blocked if products belong to it).
    /// </summary>
    /// <param name="id">Category ID.</param>
    /// <returns>Success message or error if blocked.</returns>
    /// <response code="200">Category deleted successfully.</response>
    /// <response code="400">Cannot delete category with associated products.</response>
    /// <response code="404">Category not found.</response>
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

        // Business rule: Check if category has products
        var hasProducts = await _productService.HasProductsForCategoryAsync(id);
        if (hasProducts)
        {
            return BadRequest(new ApiErrorResponse
            {
                Message = $"Cannot delete category '{category.Name}' (ID {id}) because it has associated products. Please reassign or delete the products first, or mark the category as inactive."
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
