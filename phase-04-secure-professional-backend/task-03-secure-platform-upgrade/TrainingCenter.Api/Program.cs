using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using TrainingCenter.Api.Common.Middleware;
using TrainingCenter.Api.Configurations;
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

// Configure JWT Options
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtOptions.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Configure EF Core DbContext
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
        options.UseInMemoryDatabase("TechMaster_Phase04_Task01_AuthDb");
    }
});

// Register Core Authentication & Identity Services
builder.Services.AddSingleton<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Register Domain & Application Services
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IInstructorService, InstructorService>();
builder.Services.AddScoped<ITrackService, TrackService>();
builder.Services.AddScoped<ITrackSessionService, TrackSessionService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IReportService, ReportService>();

// Configure Swagger/OpenAPI with JWT Bearer Security Definition
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TechMaster Secure Training Platform API",
        Version = "v1",
        Description = "Enterprise-grade secure backend platform for TechMaster Training Center with JWT Authentication, Role-Based Access Control, and Domain Services."
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Define JWT Bearer security scheme
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Bearer token"
    });

    options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer"),
            new List<string>()
        }
    });
});

// Configure CORS
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

// Global Exception Handling Middleware
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Enable Swagger UI unconditionally
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TechMaster Secure Platform API v1.0");
    c.RoutePrefix = string.Empty; // Swagger at root
    c.DocumentTitle = "TechMaster Secure Training Platform - API Explorer";
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health Check Endpoint
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

// Ensure Database & Initial Seed Data
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
            // For task 01 in local dev/testing fallback, EnsureCreated or Migrate
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
