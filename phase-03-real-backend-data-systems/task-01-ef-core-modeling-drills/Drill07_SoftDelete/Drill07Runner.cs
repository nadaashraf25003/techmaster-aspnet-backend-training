using EFCoreModelingDrills.Data;
using EFCoreModelingDrills.Drill01_DbContextFirstMigration;
using Microsoft.EntityFrameworkCore;

namespace EFCoreModelingDrills.Drill07_SoftDelete;

public static class Drill07Runner
{
    public static async Task RunAsync(DrillsDbContext context)
    {
        Console.WriteLine("\n========================================================");
        Console.WriteLine("🎯 DRILL 07: Soft Delete & Global Query Filter");
        Console.WriteLine("========================================================");

        // Create a temporary student to soft-delete
        var tempStudent = new Student
        {
            FullName = "Temp Student for Soft Delete",
            Email = $"temp.{Guid.NewGuid():N}@example.com",
            PhoneNumber = "+201099990000",
            IsActive = true
        };

        context.Students.Add(tempStudent);
        await context.SaveChangesAsync();
        int tempId = tempStudent.Id;
        Console.WriteLine($"[Created] Temporary Student #{tempId} added.");

        // Perform Soft Delete
        tempStudent.IsDeleted = true;
        tempStudent.DeletedAt = DateTime.UtcNow;
        tempStudent.IsActive = false;
        await context.SaveChangesAsync();
        Console.WriteLine($"[Soft Deleted] Student #{tempId} marked IsDeleted = true, DeletedAt = {tempStudent.DeletedAt:u}");

        // Query with default Global Query Filter
        var foundWithFilter = await context.Students.FirstOrDefaultAsync(s => s.Id == tempId);
        Console.WriteLine($"[Default Query Result] Student #{tempId} found: {foundWithFilter != null} (Expected: False due to filter)");

        // Query with IgnoreQueryFilters
        var foundWithoutFilter = await context.Students
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == tempId);
        Console.WriteLine($"[IgnoreQueryFilters Result] Student #{tempId} found: {foundWithoutFilter != null} (IsDeleted: {foundWithoutFilter?.IsDeleted})");

        Console.WriteLine("✅ Drill 07 passed: Global query filter transparently hides soft-deleted entities.");
    }
}
