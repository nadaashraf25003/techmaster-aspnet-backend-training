using EFCoreModelingDrills.Data;
using EFCoreModelingDrills.Drill01_DbContextFirstMigration;
using EFCoreModelingDrills.Drill02_OneToOneStudentProfile;
using EFCoreModelingDrills.Drill03_OneToManyInstructorTracks;
using EFCoreModelingDrills.Drill04_ManyToManyEnrollment;
using EFCoreModelingDrills.Drill05_PaymentSummary;
using EFCoreModelingDrills.Drill06_SeedData;
using EFCoreModelingDrills.Drill07_SoftDelete;
using EFCoreModelingDrills.Drill08_AuditFields;
using EFCoreModelingDrills.Drill09_ProjectionDTO;
using EFCoreModelingDrills.Drill10_Pagination;
using Microsoft.EntityFrameworkCore;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("================================================================================");
Console.WriteLine("🚀 TechMaster Academy - Phase 03: Task 01 - EF Core Modeling Drills (10 Drills)");
Console.WriteLine("================================================================================");

// Configure SQL Server LocalDB instance
var connectionString = "Server=(localdb)\\mssqllocaldb;Database=TechMaster_Drills_Db;Trusted_Connection=True;TrustServerCertificate=True;";
var options = new DbContextOptionsBuilder<DrillsDbContext>()
    .UseSqlServer(connectionString)
    .EnableSensitiveDataLogging()
    .Options;

using var context = new DrillsDbContext(options);
await context.Database.EnsureDeletedAsync();
await context.Database.EnsureCreatedAsync();

try
{
    // Execute all 10 Drills sequentially
    await Drill01Runner.RunAsync(context);
    await Drill02Runner.RunAsync(context);
    await Drill03Runner.RunAsync(context);
    await Drill04Runner.RunAsync(context);
    await Drill05Runner.RunAsync(context);
    await Drill06Runner.RunAsync(context);
    await Drill07Runner.RunAsync(context);
    await Drill08Runner.RunAsync(context);
    await Drill09Runner.RunAsync(context);
    await Drill10Runner.RunAsync(context);

    Console.WriteLine("\n================================================================================");
    Console.WriteLine("🎉 ALL 10 EF CORE MODELING DRILLS COMPLETED SUCCESSFULLY WITH 100% PASS RATE!");
    Console.WriteLine("================================================================================");
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\n❌ An error occurred while executing the drills: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
    Console.ResetColor();
}
