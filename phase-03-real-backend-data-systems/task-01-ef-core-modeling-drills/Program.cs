using EFCoreModelingDrills.Data;
using EFCoreModelingDrills.Drill01_DbContextFirstMigration;
using EFCoreModelingDrills.Drill02_OneToOneStudentProfile;
using EFCoreModelingDrills.Drill03_OneToManyInstructorTracks;
using Microsoft.EntityFrameworkCore;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("================================================================================");
Console.WriteLine("🚀 TechMaster Academy - Phase 03: Task 01 - EF Core Modeling Drills (10 Drills)");
Console.WriteLine("================================================================================");

// Configure in-memory database instance seeded with deterministic test data for instant verification
var options = new DbContextOptionsBuilder<DrillsDbContext>()
    .UseInMemoryDatabase(databaseName: "TechMaster_Drills_Db_" + Guid.NewGuid())
    .EnableSensitiveDataLogging()
    .Options;

using var context = new DrillsDbContext(options);
await context.Database.EnsureCreatedAsync();

try
{
    // Execute all 10 Drills sequentially
    await Drill01Runner.RunAsync(context);
    await Drill02Runner.RunAsync(context);
    await Drill03Runner.RunAsync(context);

}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\n❌ An error occurred while executing the drills: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
    Console.ResetColor();
}
