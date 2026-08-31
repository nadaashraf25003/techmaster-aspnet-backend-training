using Microsoft.AspNetCore.Mvc;
using ProductsCategoriesApi.DTOs;
using ProductsCategoriesApi.Services;

namespace ProductsCategoriesApi.Controllers;

/// <summary>
/// API Controller for managing store products, inventory stock, search/filtering, and stock reporting.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// Feature 03: Return products with search, category, price range, and availability filters.
    /// </summary>
    /// <param name="query">Filter parameters.</param>
    /// <returns>Filtered list of products.</returns>
    /// <response code="200">Products retrieved successfully.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] ProductFilterQuery query)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var products = await _productService.GetFilteredProductsAsync(query);
        return Ok(products);
    }

    /// <summary>
    /// Feature 03: Return products with low stock (below or equal to threshold).
    /// </summary>
    /// <param name="threshold">Stock threshold (defaults to 5).</param>
    /// <returns>List of low stock products.</returns>
    /// <response code="200">Low stock products retrieved successfully.</response>
    [HttpGet("low-stock")]
    [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLowStock([FromQuery] int threshold = 5)
    {
        if (threshold < 1)
        {
            return BadRequest(new ApiErrorResponse { Message = "Threshold must be at least 1." });
        }

        var products = await _productService.GetLowStockProductsAsync(threshold);
        return Ok(products);
    }

    /// <summary>
    /// Feature 04: Stock Value and Grouped Reports.
    /// Returns total stock valuation, category summaries, out of stock, and low stock items.
    /// </summary>
    /// <returns>Comprehensive stock report.</returns>
    /// <response code="200">Stock report generated successfully.</response>
    [HttpGet("reports/stock-value")]
    [ProducesResponseType(typeof(StockReportResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStockValueReport()
    {
        var report = await _productService.GetStockReportAsync();
        return Ok(report);
    }

    /// <summary>
    /// Feature 02: Return a single product by ID.
    /// </summary>
    /// <param name="id">Product unique ID.</param>
    /// <returns>Product details.</returns>
    /// <response code="200">Product found and returned.</response>
    /// <response code="404">Product with specified ID was not found.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound(new ApiErrorResponse { Message = $"Product with ID {id} was not found." });
        }

        return Ok(product);
    }

    /// <summary>
    /// Feature 02: Create a new product with category validation.
    /// </summary>
    /// <param name="request">Product creation payload.</param>
    /// <returns>The created product.</returns>
    /// <response code="201">Product created successfully.</response>
    /// <response code="400">Invalid payload, invalid category, or negative price/stock.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, error, data) = await _productService.CreateProductAsync(request);
        if (!success)
        {
            return BadRequest(new ApiErrorResponse { Message = error ?? "Failed to create product." });
        }

        return CreatedAtAction(nameof(GetById), new { id = data!.ProductId }, data);
    }

    /// <summary>
    /// Feature 02: Update an existing product.
    /// </summary>
    /// <param name="id">Product ID.</param>
    /// <param name="request">Updated product payload.</param>
    /// <returns>The updated product.</returns>
    /// <response code="200">Product updated successfully.</response>
    /// <response code="400">Invalid category or payload data.</response>
    /// <response code="404">Product not found.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, error, data) = await _productService.UpdateProductAsync(id, request);
        if (!success)
        {
            if (error != null && error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new ApiErrorResponse { Message = error });
            }

            return BadRequest(new ApiErrorResponse { Message = error ?? "Failed to update product." });
        }

        return Ok(data);
    }

    /// <summary>
    /// Feature 02 / Stock: Update stock quantity for a product.
    /// </summary>
    /// <param name="id">Product ID.</param>
    /// <param name="request">New stock quantity payload.</param>
    /// <returns>The updated product.</returns>
    /// <response code="200">Stock updated successfully.</response>
    /// <response code="400">Invalid stock quantity.</response>
    /// <response code="404">Product not found.</response>
    [HttpPatch("{id:int}/stock")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStock([FromRoute] int id, [FromBody] UpdateStockRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, error, data) = await _productService.UpdateStockAsync(id, request.StockQuantity);
        if (!success)
        {
            return NotFound(new ApiErrorResponse { Message = error ?? $"Product with ID {id} was not found." });
        }

        return Ok(data);
    }

    /// <summary>
    /// Feature 02: Delete a product.
    /// </summary>
    /// <param name="id">Product ID.</param>
    /// <returns>Success confirmation.</returns>
    /// <response code="200">Product deleted successfully.</response>
    /// <response code="404">Product not found.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var (success, error) = await _productService.DeleteProductAsync(id);
        if (!success)
        {
            return NotFound(new ApiErrorResponse { Message = error ?? $"Product with ID {id} was not found." });
        }

        return Ok(new ApiResponse
        {
            Success = true,
            Message = $"Product with ID {id} was successfully deleted."
        });
    }
}
