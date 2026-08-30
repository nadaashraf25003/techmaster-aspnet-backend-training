using ApiRoutingDrills.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ApiRoutingDrills.Controllers;

/// <summary>
/// Drill 15: Standard Error Shape Demonstration
/// </summary>
[ApiController]
[Route("api/errors")]
public class ErrorsDemoController : ControllerBase
{
    [HttpGet("demo")]
    public IActionResult GetErrorDemo([FromQuery] string type = "bad-request")
    {
        switch (type.ToLowerInvariant())
        {
            case "not-found":
                return NotFound(new StandardErrorResponse
                {
                    Success = false,
                    Message = "The requested resource could not be found.",
                    Errors = new List<string> { "Resource with specified identifier does not exist." },
                    StatusCode = StatusCodes.Status404NotFound,
                    Timestamp = DateTime.UtcNow
                });

            case "validation":
                return BadRequest(new StandardErrorResponse
                {
                    Success = false,
                    Message = "One or more validation errors occurred.",
                    Errors = new List<string>
                    {
                        "Field 'Title' is required.",
                        "Field 'Email' must be a valid email format.",
                        "Field 'Age' must be between 18 and 100."
                    },
                    StatusCode = StatusCodes.Status400BadRequest,
                    Timestamp = DateTime.UtcNow
                });

            case "bad-request":
            default:
                return BadRequest(new StandardErrorResponse
                {
                    Success = false,
                    Message = "Invalid request parameters provided.",
                    Errors = new List<string> { "Parameter 'type' was invalid or triggered bad request demo." },
                    StatusCode = StatusCodes.Status400BadRequest,
                    Timestamp = DateTime.UtcNow
                });
        }
    }
}
