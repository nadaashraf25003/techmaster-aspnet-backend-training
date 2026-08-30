using ApiRoutingDrills.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiRoutingDrills.Controllers;

/// <summary>
/// Drill 04: Temperature Conversion API
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ConverterController : ControllerBase
{
    private readonly IConverterService _converterService;

    public ConverterController(IConverterService converterService)
    {
        _converterService = converterService;
    }

    [HttpGet("celsius-to-fahrenheit")]
    public IActionResult CelsiusToFahrenheit([FromQuery] decimal value)
    {
        var (fahrenheit, formula) = _converterService.ConvertCelsiusToFahrenheit(value);

        return Ok(new
        {
            celsius = value,
            fahrenheit,
            formulaUsed = formula
        });
    }
}
