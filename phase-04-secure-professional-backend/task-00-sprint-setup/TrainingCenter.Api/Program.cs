using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common.Middleware;
using TrainingCenter.Api.Data;
using TrainingCenter.Api.Services.Implementations;
using TrainingCenter.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers with String Enum serialization
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Configure EF Core DbContext with SQL Server connection and resilient retry policy
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<TrainingCenterDbContext>(options =>
{
    if (!string.IsNullOrWhiteSpace(defaultConnection) && !builder.Environment.IsEnvironment("Testing"))
    {
        options.UseSqlServer(defaultConnection, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);
            sqlOptions.MigrationsAssembly(typeof(TrainingCenterDbContext).Assembly.FullName);
        });
    }
    else
    {
        options.UseInMemoryDatabase("TechMaster_Phase03_Task06_ProductionDb");
    }
});

// Register Domain & Application Services
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IInstructorService, InstructorService>();
builder.Services.AddScoped<ITrackService, TrackService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IReportService, ReportService>();

// Configure Swagger/OpenAPI (Enabled for Production & Development)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Configure CORS for web clients & external callers
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

// Configure the HTTP request pipeline
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Enable Swagger and Swagger UI unconditionally (Production & Development)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TechMaster Phase 03 - Task 06 Production API v1.0");
    c.RoutePrefix = string.Empty; // Swagger UI served at root (http://domain.com/)
    c.DocumentTitle = "TechMaster Training Center API - Live Swagger";
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Health check endpoint mapping
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

// Ensure Database Migrations & Initial Seed Data
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
            await context.Database.MigrateAsync();
        }
        else
        {
            await context.Database.EnsureCreatedAsync();
        }

        await DbInitializer.SeedAsync(context);
        logger.LogInformation("Database initialized and verified successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An unexpected error occurred during database migration/seeding.");
    }
}

app.Run();
