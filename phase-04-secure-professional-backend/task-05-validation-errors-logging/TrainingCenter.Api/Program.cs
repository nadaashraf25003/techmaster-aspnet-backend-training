using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Data;
using TrainingCenter.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure Controllers with String Enum conversion & Uniform Validation
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddApiValidationHandling();

// 2. Configure Database Context & Resilient Connections
builder.Services.AddDatabase(builder.Configuration, builder.Environment);

// 3. Configure JWT Authentication & Authorization Policies
builder.Services.AddJwtAuthentication(builder.Configuration);

// 4. Configure Application & Domain Services (IoC Container)
builder.Services.AddApplicationServices();

// 5. Configure Swagger / OpenAPI Documentation with JWT Security
builder.Services.AddSwaggerDocumentation();

// 6. Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 7. Custom Middleware Pipeline (CorrelationId -> Logging -> Exception Handling)
app.UseCustomMiddlewares();

// 8. Swagger Documentation UI
app.UseSwaggerDocumentation();

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 9. Liveness & Readiness Healthcheck Endpoint
app.MapGet("/health", async (TrainingCenterDbContext context) =>
{
    var canConnect = await context.Database.CanConnectAsync();
    return Results.Ok(new
    {
        Status = canConnect ? "Healthy" : "Degraded",
        DatabaseConnected = canConnect,
        DatabaseProvider = context.Database.ProviderName ?? "Unknown",
        TimestampUtc = DateTime.UtcNow
    });
});

// 10. Database Seeding & Schema Migration
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var context = services.GetRequiredService<TrainingCenterDbContext>();

    try
    {
        logger.LogInformation("Ensuring database is up to date...");
        if (context.Database.IsSqlServer())
        {
            try
            {
                await context.Database.MigrateAsync();
            }
            catch
            {
                await context.Database.EnsureCreatedAsync();
            }
        }
        else
        {
            await context.Database.EnsureCreatedAsync();
        }

        await DbInitializer.SeedAsync(context);
        logger.LogInformation("Database initialized and seeded successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database migration/seeding.");
    }
}

app.Run();
