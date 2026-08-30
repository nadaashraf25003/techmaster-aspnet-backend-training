using Microsoft.AspNetCore.Mvc;

namespace ApiRoutingDrills.Controllers;

/// <summary>
/// Drill 03: Query String Calculator
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CalculatorController : ControllerBase
{
    [HttpGet("add")]
    public IActionResult Add([FromQuery] decimal a, [FromQuery] decimal b)
    {
        decimal result = a + b;

        return Ok(new
        {
            a,
            b,
            operation = "add",
            result
        });
    }
}
