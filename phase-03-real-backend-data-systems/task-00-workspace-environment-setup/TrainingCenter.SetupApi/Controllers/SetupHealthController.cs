using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenter.SetupApi.Data;

namespace TrainingCenter.SetupApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SetupHealthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public SetupHealthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    /// <summary>
    /// Verifies API health and environment readiness.
    /// </summary>
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        return Ok(new
        {
            success = true,
            message = "TechMaster Training Center Setup API is running.",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
            timestamp = DateTime.UtcNow,
            efCoreConfigured = true,
            provider = _context.Database.ProviderName
        });
    }

    /// <summary>
    /// Checks database connectivity safely without exposing connection secrets.
    /// </summary>
    [HttpGet("db-check")]
    public async Task<IActionResult> CheckDatabaseConnection()
    {
        try
        {
            bool canConnect = await _context.Database.CanConnectAsync();
            var pendingMigrations = (await _context.Database.GetPendingMigrationsAsync()).ToList();
            var appliedMigrations = (await _context.Database.GetAppliedMigrationsAsync()).ToList();

            return Ok(new
            {
                success = true,
                canConnect,
                appliedMigrationsCount = appliedMigrations.Count,
                appliedMigrations,
                pendingMigrationsCount = pendingMigrations.Count,
                pendingMigrations,
                databaseProvider = _context.Database.ProviderName
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Database connection check encountered an issue.",
                error = ex.Message
            });
        }
    }
}
