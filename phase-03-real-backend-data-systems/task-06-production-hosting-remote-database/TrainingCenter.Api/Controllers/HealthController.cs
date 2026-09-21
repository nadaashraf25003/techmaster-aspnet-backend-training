using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Data;
using TrainingCenter.Api.DTOs.Common;

namespace TrainingCenter.Api.Controllers;

/// <summary>
/// Health check and production connectivity diagnostic endpoint.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HealthController : ControllerBase
{
    private readonly TrainingCenterDbContext _context;
    private readonly IWebHostEnvironment _env;

    public HealthController(TrainingCenterDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    /// <summary>
    /// Checks database connectivity, provider status, and environment health.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetHealth()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            var providerName = _context.Database.ProviderName ?? "Unknown";
            var studentsCount = canConnect ? await _context.Students.CountAsync() : 0;
            var tracksCount = canConnect ? await _context.TrainingTracks.CountAsync() : 0;

            var healthData = new
            {
                Status = canConnect ? "Healthy" : "Degraded",
                Environment = _env.EnvironmentName,
                DatabaseProvider = providerName,
                DatabaseConnected = canConnect,
                ActiveRecords = new
                {
                    Students = studentsCount,
                    Tracks = tracksCount
                },
                ServerTimeUtc = DateTime.UtcNow,
                Version = "1.0.0-phase03-task06"
            };

            if (!canConnect)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, 
                    ApiResponse<object>.FailureResponse("Database connection failed", new List<string> { "Unable to connect to remote SQL database" }, 503));
            }

            return Ok(ApiResponse<object>.SuccessResponse(healthData, "TechMaster Training Center API is healthy and operational online."));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                ApiResponse<object>.FailureResponse("Health check failed", new List<string> { ex.Message }, 503));
        }
    }
}
