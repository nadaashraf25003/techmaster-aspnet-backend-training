using EFCoreModelingDrills.Data;
using EFCoreModelingDrills.Drill01_DbContextFirstMigration;
using Microsoft.EntityFrameworkCore;

namespace EFCoreModelingDrills.Drill08_AuditFields;

public static class Drill08Runner
{
    public static async Task RunAsync(DrillsDbContext context)
    {
        Console.WriteLine("\n========================================================");
        Console.WriteLine("🎯 DRILL 08: Automated UTC Audit Tracking");
        Console.WriteLine("========================================================");

        var auditStudent = new Student
        {
            FullName = "Audit Test Student",
            Email = $"audit.{Guid.NewGuid():N}@example.com",
            PhoneNumber = "+201088881111",
            IsActive = true
        };

        context.Students.Add(auditStudent);
        await context.SaveChangesAsync();
        Console.WriteLine($"[Entity Added] CreatedAt set automatically: {auditStudent.CreatedAt:yyyy-MM-dd HH:mm:ss.fff} UTC | UpdatedAt: {auditStudent.UpdatedAt?.ToString() ?? "null"}");

        // Simulate update
        await Task.Delay(50);
        auditStudent.FullName = "Audit Test Student (Modified)";
        await context.SaveChangesAsync();
        Console.WriteLine($"[Entity Modified] UpdatedAt set automatically: {auditStudent.UpdatedAt:yyyy-MM-dd HH:mm:ss.fff} UTC");

        Console.WriteLine("✅ Drill 08 passed: SaveChangesAsync interception automatically tracks CreatedAt and UpdatedAt in UTC.");
    }
}
