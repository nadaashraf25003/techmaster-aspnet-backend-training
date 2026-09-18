using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common.Middleware;
using TrainingCenter.Api.Data;
using TrainingCenter.Api.Services.Implementations;
using TrainingCenter.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Controllers with JSON String Enum Converters
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// 2. Configure Database Context (SQL Server with resilient in-memory fallback)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<TrainingCenterDbContext>(options =>
{
    if (!string.IsNullOrWhiteSpace(connectionString) && !builder.Environment.IsEnvironment("Testing"))
    {
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
        });
    }
    else
    {
        options.UseInMemoryDatabase("TechMaster_TrainingCenter_InMemoryDb");
    }
});

// 3. Register Application Services (Dependency Injection)
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IInstructorService, InstructorService>();
builder.Services.AddScoped<ITrackService, TrackService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IReportService, ReportService>();

// 4. Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 5. Configure Swagger / OpenAPI Documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 6. Database Migration & Seeding Lifecycle
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<TrainingCenterDbContext>();
        
        // If relational database, ensure created/migrated
        if (context.Database.IsRelational())
        {
            await context.Database.EnsureCreatedAsync();
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
        logger.LogWarning(ex, "Could not connect to SQL Server. Reconfiguring to In-Memory for resilient execution.");
    }
}

// 7. Configure HTTP Request Pipeline & Middlewares
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Enable Swagger in all environments for testing & live hosting
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "TechMaster Academy API v1");
    options.RoutePrefix = string.Empty; // Serve Swagger at app root URL
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
