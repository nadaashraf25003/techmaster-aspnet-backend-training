using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common.Responses;
using TrainingCenter.Api.Data;
using TrainingCenter.Api.Services.Implementations;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Core Identity & Token Services
        services.AddSingleton<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();

        // Domain & Application Services
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IInstructorService, InstructorService>();
        services.AddScoped<ITrackService, TrackService>();
        services.AddScoped<ITrackSessionService, TrackSessionService>();
        services.AddScoped<IEnrollmentService, EnrollmentService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IReportService, ReportService>();

        return services;
    }

    public static IServiceCollection AddApiValidationHandling(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => !string.IsNullOrWhiteSpace(x.ErrorMessage) ? x.ErrorMessage : x.Exception?.Message ?? "Invalid value provided.")
                    .Distinct()
                    .ToList();

                var response = ApiResponse<object>.FailureResponse("Validation failed. Please correct the highlighted errors.", errors, StatusCodes.Status400BadRequest);
                return new BadRequestObjectResult(response);
            };
        });

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var defaultConnection = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<TrainingCenterDbContext>(options =>
        {
            if (!string.IsNullOrWhiteSpace(defaultConnection) && !environment.IsEnvironment("Testing"))
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
                options.UseInMemoryDatabase("TechMaster_Phase04_Task05_ValidationDb");
            }
        });

        return services;
    }
}
