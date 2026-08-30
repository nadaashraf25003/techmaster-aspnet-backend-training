using Microsoft.AspNetCore.Mvc;

namespace ApiRoutingDrills.Controllers;

/// <summary>
/// Drill 02: Route Parameter Echo
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ToolsController : ControllerBase
{
    [HttpGet("echo/{name}")]
    public IActionResult Echo([FromRoute] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(new { error = "Name parameter cannot be empty or whitespace." });
        }

        return Ok(new
        {
            originalName = name,
            message = $"Hello, {name}!"
        });
    }
}
