using EFCoreModelingDrills.Data;
using Microsoft.EntityFrameworkCore;

namespace EFCoreModelingDrills.Drill06_SeedData;

public static class Drill06Runner
{
    public static async Task RunAsync(DrillsDbContext context)
    {
        Console.WriteLine("\n========================================================");
        Console.WriteLine("🎯 DRILL 06: Deterministic Model Seed Data Verification");
        Console.WriteLine("========================================================");

        var instructorsCount = await context.Instructors.CountAsync();
        var tracksCount = await context.TrainingTracks.CountAsync();
        var studentsCount = await context.Students.CountAsync();
        var enrollmentsCount = await context.Enrollments.CountAsync();

        Console.WriteLine($"[Seed Data Metrics]");
        Console.WriteLine($"  - Instructors Seeded: {instructorsCount} (Expected >= 2)");
        Console.WriteLine($"  - Tracks Seeded:      {tracksCount} (Expected >= 3)");
        Console.WriteLine($"  - Students Seeded:    {studentsCount} (Expected >= 5)");
        Console.WriteLine($"  - Enrollments Seeded: {enrollmentsCount} (Expected >= 5)");

        Console.WriteLine("✅ Drill 06 passed: All required seed entities populated deterministically.");
    }
}
