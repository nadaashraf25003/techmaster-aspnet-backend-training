using Microsoft.AspNetCore.Mvc;

namespace ApiRoutingDrills.Controllers;

/// <summary>
/// Drill 14: Explicit Status Code Practice (200, 201, 204, 400, 404)
/// </summary>
[ApiController]
[Route("api/status-codes")]
public class StatusCodesController : ControllerBase
{
    /// <summary>
    /// Demonstrates HTTP 200 OK
    /// </summary>
    [HttpGet("200-ok")]
    public IActionResult GetOk()
    {
        return Ok(new
        {
            statusCode = 200,
            status = "OK",
            description = "Standard success response indicating that the request has succeeded."
        });
    }

    /// <summary>
    /// Demonstrates HTTP 201 Created
    /// </summary>
    [HttpPost("201-created")]
    public IActionResult GetCreated()
    {
        var sampleResource = new
        {
            id = 101,
            name = "Demo Item",
            createdAt = DateTime.UtcNow
        };

        return Created($"/api/status-codes/resource/{sampleResource.id}", new
        {
            statusCode = 201,
            status = "Created",
            description = "Returned when a new resource is successfully created on the server.",
            data = sampleResource
        });
    }

    /// <summary>
    /// Demonstrates HTTP 204 No Content
    /// </summary>
    [HttpDelete("204-no-content")]
    public IActionResult GetNoContent()
    {
        // 204 indicates request succeeded but there is no body to return (standard for DELETE)
        return NoContent();
    }

    /// <summary>
    /// Demonstrates HTTP 400 Bad Request
    /// </summary>
    [HttpGet("400-bad-request")]
    public IActionResult GetBadRequest([FromQuery] string? sampleInput)
    {
        return BadRequest(new
        {
            statusCode = 400,
            status = "Bad Request",
            description = "Returned when the client request is invalid, missing required fields, or fails business validation.",
            inputReceived = sampleInput
        });
    }

    /// <summary>
    /// Demonstrates HTTP 404 Not Found
    /// </summary>
    [HttpGet("404-not-found")]
    public IActionResult GetNotFound([FromQuery] int nonExistentId = 999)
    {
        return NotFound(new
        {
            statusCode = 404,
            status = "Not Found",
            description = "Returned when the requested resource or route cannot be found on the server.",
            requestedId = nonExistentId
        });
    }
}
