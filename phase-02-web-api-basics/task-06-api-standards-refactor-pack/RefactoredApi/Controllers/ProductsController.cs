using Microsoft.AspNetCore.Mvc;
using RefactoredApi.DTOs;
using RefactoredApi.Services;

namespace RefactoredApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var products = _productService.GetAll();
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
        var product = _productService.GetById(id);
        if (product == null)
        {
            return NotFound(new { message = $"Product with ID {id} was not found." });
        }

        return Ok(product);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] CreateProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { message = "Product name cannot be empty." });
        }

        if (request.Price < 0)
        {
            return BadRequest(new { message = "Price must be a non-negative value." });
        }

        if (request.Stock < 0)
        {
            return BadRequest(new { message = "Stock cannot be negative." });
        }

        var created = _productService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
