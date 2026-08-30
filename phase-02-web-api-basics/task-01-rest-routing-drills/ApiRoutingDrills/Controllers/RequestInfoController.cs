using Microsoft.AspNetCore.Mvc;

namespace ApiRoutingDrills.Controllers;

/// <summary>
/// Drill 13: Header Reader Endpoint
/// </summary>
[ApiController]
[Route("api/request-info")]
public class RequestInfoController : ControllerBase
{
    [HttpGet]
    public IActionResult GetRequestInfo()
    {
        if (!Request.Headers.TryGetValue("X-Student-Name", out var studentName) || string.IsNullOrWhiteSpace(studentName))
        {
            return BadRequest(new
            {
                error = "Header 'X-Student-Name' is required."
            });
        }

        return Ok(new
        {
            studentName = studentName.ToString(),
            requestPath = Request.Path.Value,
            timestamp = DateTime.UtcNow
        });
    }
}
