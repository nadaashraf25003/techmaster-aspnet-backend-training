using Microsoft.AspNetCore.Mvc;

namespace ApiRoutingDrills.Controllers;

/// <summary>
/// Drill 05: Grade Calculation API
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GradesController : ControllerBase
{
    [HttpGet("calculate")]
    public IActionResult CalculateGrade([FromQuery] decimal score)
    {
        if (score < 0 || score > 100)
        {
            return BadRequest(new { error = "Score must be between 0 and 100." });
        }

        string grade;
        string status = score >= 60 ? "Pass" : "Fail";

        if (score >= 90) grade = "A";
        else if (score >= 80) grade = "B";
        else if (score >= 70) grade = "C";
        else if (score >= 60) grade = "D";
        else grade = "F";

        return Ok(new
        {
            score,
            grade,
            status
        });
    }
}
